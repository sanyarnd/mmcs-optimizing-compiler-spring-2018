using System;
using System.Threading;
using System.Threading.Tasks;
using Compiler.ILcodeGenerator;
using Compiler.ThreeAddrCode;

namespace Compiler.IDE.Handlers
{
    internal class IlCodeHandler
    {
        public event EventHandler<TAcode2ILcodeTranslator> GenerationCompleted = delegate { };
        public event EventHandler Aborted = delegate { };
        public event EventHandler<Exception> RuntimeErrored = delegate { };
        public event EventHandler AlreadyRunningErrored = delegate { };
        public event EventHandler RuntimeStarted = delegate { };

        private TAcode2ILcodeTranslator _ilProgram;
        private CancellationTokenSource ts;
        private Task _runTask;

        public void GenerateIlCode(TACode code)
        {
            var trans = new TAcode2ILcodeTranslator();
            trans.Translate(code);
            _ilProgram = trans;
            GenerationCompleted(null, trans);
        }

        public void Run()
        {
            if (_runTask != null && _runTask.Status == TaskStatus.Running)
            {
                AlreadyRunningErrored(null, null);
                return;
            }

            RuntimeStarted(null, null);

            ts = new CancellationTokenSource();
            
            _runTask = Task.Factory.StartNew(() =>
            {
                Thread t = Thread.CurrentThread;
                using (ts.Token.Register(t.Abort))
                {
                    _ilProgram.RunProgram();
                }
            }, ts.Token);
        }

        public void Abort()
        {
            if (_runTask != null && _runTask.Status == TaskStatus.Running)
            {
                ts?.Cancel();
                Aborted(null, null);
            }
        }
    }
}