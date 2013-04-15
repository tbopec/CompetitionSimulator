using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIRLab.Mathematics;
using System.IO;
using System.Drawing;
using AIRLab.Drawing;
using AIRLab.Thornado;
using AIRLab.Thornado.IOs;
using System.Runtime.Serialization.Formatters.Binary;

namespace RoboCoP.Plus.Common {

    public class RecognizerSettings : ServiceSettings, ICheckable {

        [ThornadoField("", typeof(IntIO))]
        public int CellSize { get; set; }

        [ThornadoField("")]
        public bool UpdateRulesAtStartup { get; set; }
        [ThornadoField("")]
        public string SamplesFile { get; set; }
        [ThornadoField("")]
        public string RulesFile { get; set; }

        public RecognizerSettings() {
            SamplesFile = "";
            RulesFile = "";
        }

        public void CheckSelfConsistancy(LogicErrorList list) {
            if (CellSize == 0)
                list.Add(LogicErrorLevel.Error, "Cell size can not be 0");
        }

    }

    public abstract class Recognizer<T>
        where T : RecognizerSettings, new() {

        protected TerminalServiceApp<T> App;

        public Recognizer(string name, string[] args) {
            App = new TerminalServiceApp<T>(name, args, false, 1, 1);
            //App.Service.Out[0].BufferSize = 1000000;
            App.RegisterKey(ConsoleKey.Q, "Update rules");
            App.KeyPressed += new Action<ConsoleKeyInfo>(KeyPressed);
            //App.Service.Com.CommandReceived += new Action<Command>(CommandReceived);
            if (App.Service.Com != null && App.Service.Com["Recognizer"] != null)
            {
                App.Service.Com["Recognizer"].AddSignalListener("UpdateRules", UpdateRulesSignalReceived);
                App.Service.Com["Recognizer"].AddSignalListener("SaveRules", SaveRulesSignalReceived);
            }
        }

        void UpdateRulesSignalReceived() {
            App.Info("Updating rules (signal caught)...:");
            UpdateRules();
            App.Info("Done");
        }

        void SaveRulesSignalReceived() {
            App.Info("Saving rules (signal caught)...:");
            SaveRules();
            App.Info("Done");
        }

        void KeyPressed(ConsoleKeyInfo obj) {
            if (obj.Key == ConsoleKey.Q) {
                App.Info("Updating rules (key pressed)...");
                UpdateRules();
                App.Info("Done");
            }
            if (obj.Key == ConsoleKey.W) {
                App.Info("Saving rules (key pressed)...");
                SaveRules();
                App.Info("Done");
            }
        }

        void UpdateRules() {
            RecognitionBase bs = null;
            App.ReadLocalFile(App.Settings.SamplesFile, "Base with samples",
                delegate(FileStream inf) {
                    bs = (RecognitionBase)((new BinaryFormatter()).Deserialize(inf));

                });
            var back = new AsyncCallback(delegate { });
            (new Action<RecognitionBase>(UpdateRules)).BeginInvoke(bs, back, null);
        }

        void SaveRules() {
            App.WriteLocalFile(App.Settings.RulesFile, "Rules",
                delegate(FileStream stream) {
                    (new BinaryFormatter()).Serialize(stream, Rules);
                });
        }

        protected abstract void Analyze(FastBitmap fbmp, ClassifiedBitmap cl);

        protected virtual void ImageProcessed(Bitmap bmp, ClassifiedBitmap cl) { }

        protected virtual void UpdateRules(RecognitionBase samples) { }

        protected abstract object Rules { get; set; }

        public void Run() {
            if (App.Settings.UpdateRulesAtStartup)
                UpdateRules();
            else
                App.ReadLocalFile(App.Settings.RulesFile, "Rules",
                    delegate(FileStream stream) {
                        Rules = (new BinaryFormatter()).Deserialize(stream);
                    });

            while (true) {
                //byte[] message = App.Service.Receivers[0].Receive().Body;
                byte[] message = App.Service.In[0].ReceiveBinary();
                App.Log("Image received");
                MemoryStream stream = new MemoryStream(message);
                var bitmap = (Bitmap)Bitmap.FromStream(stream);
                var original = FastBitmap.FromBitmap(bitmap);
                var cl = new ClassifiedBitmap(bitmap.Width, bitmap.Height, App.Settings.CellSize);
                App.Log("Conversion to FastBitmap");
                if (Rules != null)
                    Analyze(original, cl);
                else
                    App.Error("Analyzis impossible, rules are not loaded");
                App.Log("Analyzis complete");
                
                App.Service.Out[0].SendBinary(cl.Write());
                App.Log("Image sent");
                App.EndCycle();
                ImageProcessed(bitmap, cl);
            }
        }

    }

    public abstract class TrivialRecognizer<T> : Recognizer<T>
        where T : RecognizerSettings, new() {

        Vector result;

        public TrivialRecognizer(string name, string[] args)
            : base(name, args) {
            result = new RealVector(0);
        }

        protected abstract int GetClass(Color c);

        protected override void Analyze(FastBitmap original, ClassifiedBitmap cl) {
            for (int x = 0; x < cl.OriginalWidth; x += cl.CellSize)
                for (int y = 0; y < cl.OriginalHeight; y += cl.CellSize) {
                    result.SetConstant(0);
                    for (int xx = 0; xx < cl.CellSize; xx++)
                        for (int yy = 0; yy < cl.CellSize; yy++) {
                            if (x + xx >= cl.OriginalWidth || y + yy >= cl.OriginalHeight) continue;
                            var col = original.GetPixel(x + xx, y + yy);
                            var current = GetClass(col);
                            if (current >= result.Count) {
                                var t = new RealVector(current + 1);
                                for (int i = 0; i < result.Count; i++)
                                    t[i] = result[i];
                                result = t;
                            }
                            result[current]++;
                        }
                    cl.SetByOriginal(x, y, result.GetMaxIndex());
                }
        }

    }

}
