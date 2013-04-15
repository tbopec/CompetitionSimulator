using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using AIRLab.Thornado;
using Eurosim.Core;
using Eurosim.Core.Graphics;
using Eurosim.Core.Replay;
using RoboCoP;
using RoboCoP.Plus;

namespace EurosimRePlayer
{
    public class EurosimReplayer
    {
        private static EurosimReplayerSettings Settings;
        private static BodyCollection<Body> RootBody;

        public static void Main(string[] args)
        {
            var app = new FormServiceApp<EurosimReplayerSettings>("EurosimReplayer", args);
            //Settings = IO.INI.ParseFile<EurosimReplayerSettings>(args[0], "EurosimReplayer");
            Settings = app.Settings;
            var replayPlayer = new ReplayPlayer();
            RootBody = new BodyCollection<Body>();
            var filename = Settings.FileName;
            replayPlayer.Construct(filename, RootBody);
            var playerthread = new Thread(() =>
                                              {
                                                  while (true)
                                                  {
                                                      replayPlayer.Update(Settings.DT);
                                                      Thread.Sleep(TimeSpan.FromSeconds(Settings.DT));
                                                  }
                                              });
            playerthread.IsBackground = true;
            playerthread.Start();
            var scores = new List<List<Score>>();//TODO!
            var form = new EmulatorForm(Drawer.DefaultSettings, scores, Settings.ShowControls);
            var graphicsthread = new Thread(() =>
            {
                    var drawer = Drawer.FromMode(Settings.VideoMode, RootBody, form);
                    drawer.InitializeModels(RootBody, null);//TODO!!костыль с полом
                    drawer.ShowManyFrames();
                    Application.Run(form);
                    drawer.Dispose();
            });
            graphicsthread.SetApartmentState(ApartmentState.STA);
            graphicsthread.Start();
        }

    }
}

