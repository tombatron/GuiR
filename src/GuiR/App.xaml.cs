using GuiR.Configuration;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;

namespace GuiR
{
    public partial class App : Application
    {
        public App() : base()
        {
            AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssembly;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            ServiceLocator.Dispose();
        }

        private static Assembly OnResolveAssembly(object sender, ResolveEventArgs args)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            AssemblyName assemblyName = new AssemblyName(args.Name);

            string path = $"{assemblyName.Name}.dll";

            if (assemblyName.CultureInfo.Equals(CultureInfo.InvariantCulture))
            {
                path = $"{assemblyName.Name}.dll";
            }
            else
            {
                path = $@"{assemblyName.CultureInfo}\{path}";
            }

            using (Stream stream = executingAssembly.GetManifestResourceStream(path))
            {
                if (stream == null)
                {
                    return null;
                }
                else
                {
                    var assemblyRawBytes = new byte[stream.Length];
                    stream.Read(assemblyRawBytes, 0, assemblyRawBytes.Length);
                    return Assembly.Load(assemblyRawBytes);
                }
            }
        }
    }
}
