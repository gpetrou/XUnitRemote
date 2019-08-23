

namespace XUnitRemote
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Xunit;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    [DebuggerDisplay(@"\{ class = {TestMethod.TestClass.Class.Name}, method = {TestMethod.Method.Name}, display = {DisplayName}, skip = {SkipReason} \}")]
    public class RemoteHostTestCase : LongLivedMarshalByRefObject, IXunitTestCase
    {
        private IXunitTestCase _testCase;
        public IMethodInfo Method => _testCase.Method;
        public string DisplayName => _testCase.DisplayName;
        public string SkipReason => _testCase.SkipReason;
        public Exception InitializationException => null;
        public int Timeout => 0;

        public ISourceInformation SourceInformation
        {
            get => _testCase.SourceInformation;
            set => _testCase.SourceInformation = value;
        }

        public ITestMethod TestMethod => _testCase.TestMethod;
        public object[] TestMethodArguments => _testCase.TestMethodArguments;
        public Dictionary<string, List<string>> Traits => _testCase.Traits;
        public string UniqueID => _testCase.UniqueID;

        public static string AddQuotesIfRequired(string value) =>
            !string.IsNullOrWhiteSpace(value)
                ? value.Contains(" ") && (!value.StartsWith("\"", StringComparison.Ordinal) &&
                                         !value.EndsWith("\"", StringComparison.Ordinal)) ? "\"" + value + "\"" : value
                : string.Empty;

        public Task<RunSummary> RunAsync(
            IMessageSink diagnosticMessageSink,
            IMessageBus messageBus,
            object[] constructorArguments,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
        {
            TaskCompletionSource<RunSummary> tcs = new TaskCompletionSource<RunSummary>();

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            ITypeInfo classTypeInfo = _testCase.TestMethod.TestClass.Class;
            string assemblyPath = AddQuotesIfRequired(Path.Combine(baseDirectory, classTypeInfo.Assembly.AssemblyPath));
            string typeName = classTypeInfo.Name;
            string methodName = _testCase.Method.Name;
            string outputFilePath = AddQuotesIfRequired(Path.Combine(baseDirectory, "RemoteHostOutput-" + Guid.NewGuid() + ".txt"));

            try
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    WorkingDirectory = baseDirectory,
                    FileName = AddQuotesIfRequired(Path.Combine(baseDirectory, "XUnitRemoteHost.exe")),
                    Arguments = $"{assemblyPath} {typeName} {methodName} {outputFilePath}",
                    UseShellExecute = false,
                    CreateNoWindow = false
                };

                Stopwatch stopwatch = new Stopwatch();

                int exitCode = 0;
                using (Process process = new Process())
                {
                    process.StartInfo = processStartInfo;

                    process.Start();

                    stopwatch.Start();

                    process.WaitForExit();

                    if (process.HasExited)
                    {
                        exitCode = process.ExitCode;
                    }
                }

                stopwatch.Stop();
                decimal elapsedMilliseconds = (decimal)(stopwatch.ElapsedTicks * 1000.0) / Stopwatch.Frequency;

                if (File.Exists(outputFilePath))
                {
                    XunitTest test = new XunitTest(_testCase, _testCase.DisplayName);
                    string outputFileText = File.ReadAllText(outputFilePath);
                    messageBus.QueueMessage(new TestOutput(test, outputFileText));
                    File.Delete(outputFilePath);
                }

                RunSummary runSummary = new RunSummary();
                runSummary.Failed = exitCode != 0 ? 1 : 0;
                runSummary.Skipped = _testCase.SkipReason == null ? 0 : 1;
                runSummary.Time = elapsedMilliseconds;
                runSummary.Total = 1;

                tcs.SetResult(runSummary);
            }
            catch (OperationCanceledException)
            {
                tcs.SetCanceled();
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }

            return tcs.Task;
        }

        public void Deserialize(IXunitSerializationInfo info)
        {
            _testCase = info.GetValue<IXunitTestCase>("InnerTestCase");
        }

        public void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue("InnerTestCase", _testCase);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Called by the de-serializer", error: true)]
        public RemoteHostTestCase()
        {
        }

        public RemoteHostTestCase(IXunitTestCase testCase)
        {
            _testCase = testCase;
        }
    }
}
