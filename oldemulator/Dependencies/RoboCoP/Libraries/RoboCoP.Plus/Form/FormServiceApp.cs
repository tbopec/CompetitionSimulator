using System;
using System.Diagnostics;
using System.Windows.Forms;
using AIRLab.Thornado;
using RoboCoP;

namespace RoboCoP.Plus
{
    public class FormServiceApp<T>: ServiceApp<T>
        where T: ServiceSettings, new()
    {
        private readonly string firstName;
        private Form onload;
        private TextBox text;

        public FormServiceApp(string firstName, string[] args, Action<ServiceStates> callback = null)
        {
            this.firstName = firstName;

            try {
                Init(firstName, args,
                     (Action<ServiceStates>) Delegate.Combine(callback, new Action<ServiceStates>(OnStateChange)));
            }
            catch(Exception e) {
                string msg = "";
                if(text != null) {
                    msg = text.Text;
                    onload.Close();
                }
                msg += "FAIL\r\n" + e.Message;
                CreateOnload();
                text.Text = msg;
                Application.Run(onload);
                Process.GetCurrentProcess().Kill();
            }
        }

        private void CreateOnload()
        {
            onload = new Form { Text = "Загрузка " + firstName };
            text = new TextBox { Multiline = true, ReadOnly = true, Dock = DockStyle.Fill };
            onload.Controls.Add(text);
        }

        private void OnStateChange(ServiceStates state)
        {
            switch(state) {
            case ServiceStates.ServiceAppInitStart:
                CreateOnload();
                onload.Text = "Загрузка " + firstName;
                onload.Show();

                Application.DoEvents();
                break;
            case ServiceStates.ServiceInitStart:
                onload.Text = Settings.Name;
                break;
            }

            text.Text += GetMessage(state) + "\r\n";
            if(state == ServiceStates.ServiceReady)
                onload.Close();
            else
                Application.DoEvents();
        }

        protected override void ShowHelp(HelpInfo help)
        {
           // HelpMaker<T, UniIOProvider<T>>.PrintWindowsHelp(help);
        }
    }
}