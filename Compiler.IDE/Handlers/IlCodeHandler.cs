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
        private Thread _runThread;

        public void GenerateIlCode(TACode code)
        {
            var trans = new TAcode2ILcodeTranslator();
            trans.Translate(code);
            _ilProgram = trans;
            GenerationCompleted(null, trans);
        }

        public void Run()
        {
            if (_runThread != null)
            {
                AlreadyRunningErrored(null, null);
                return;
            }

            RuntimeStarted(null, null);
            Task.Factory.StartNew(() =>
            {
                _runThread = Thread.CurrentThread;
                try
                {
                    _ilProgram.RunProgram();
                }
                catch (ThreadAbortException)
                {
                    Aborted(null, null);
                    _runThread = null;
                }
                catch (Exception ex)
                {
                    RuntimeErrored(null, ex);
                }
            });
        }

        public void Abort()
        {
            _runThread?.Abort();
        }
    }
}