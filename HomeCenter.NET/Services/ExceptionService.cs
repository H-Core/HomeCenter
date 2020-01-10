using System;
using System.Threading;
using System.Threading.Tasks;

namespace HomeCenter.NET.Services
{
    /// <summary>
    /// Responsible for handling all exceptions
    /// </summary>
    public sealed class ExceptionService
    {
        #region Properties

        private RunnerService RunnerService { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Do not call it explicitly, use <see cref="Caliburn.Micro.IoC.GetInstance"/>
        /// </summary>
        /// <param name="runnerService"></param>
        public ExceptionService(RunnerService runnerService)
        {
            RunnerService = runnerService ?? throw new ArgumentNullException(nameof(runnerService));
        }

        #endregion

        #region Public methods

        /// <summary>
        /// This method throws no exceptions.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task HandleExceptionAsync(Exception exception, CancellationToken cancellationToken = default)
        {
            try
            {
                await RunnerService.RunAsync($"print {exception.Message}", cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException)
            {
            }
            catch (ApplicationException)
            {
            }
            catch (SystemException)
            {
            }
        }

        #endregion
    }
}
