
namespace XUnitRemoteHost
{
    using System;
    using System.IO;
    using System.Reflection;

    class Program
    {
        static int Main(string[] args)
        {
            int exitCode = 0;
            int argsLength = args.Length;
            if (argsLength < 4)
            {
                Console.Error.WriteLine("Usage: {0} assemblyName typeName methodName outputFilePath", typeof(Program).GetTypeInfo().Assembly.GetName().Name);

                exitCode = -1;
                Environment.Exit(exitCode);
                return exitCode;
            }

            string assemblyName = args[0];
            string typeName = args[1];
            string methodName = args[2];
            string outputFilePath = args[3];

            object instance = null;
            string exceptionText = string.Empty;
            try
            {
                Assembly assembly = Assembly.LoadFrom(assemblyName);
                Type type = assembly.GetType(typeName);
                MethodInfo methodInfo = type.GetTypeInfo().GetDeclaredMethod(methodName);
                if (!methodInfo.IsStatic)
                {
                    instance = Activator.CreateInstance(type);
                }

                methodInfo.Invoke(instance, null);
            }
            catch (Exception ex)
            {
                if (ex is TargetInvocationException && ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }

                exceptionText = ex.ToString();

                exitCode = 1;
            }
            finally
            {
                (instance as IDisposable)?.Dispose();
            }

            if (exceptionText.Length > 0)
            {
                File.WriteAllText(outputFilePath, exceptionText);
            }

            return exitCode;
        }
    }
}
