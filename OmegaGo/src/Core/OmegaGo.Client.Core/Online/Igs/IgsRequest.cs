using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace OmegaGo.Core.Online.Igs
{
    /// <summary>
    /// Represents a single command that the CLIENT sends to the IGS SERVER. An <see cref="IgsRequest"/> instance handles this command
    /// from the moment it's submitted to the moment the server finishes sending its reply. 
    /// 
    /// This class should only ever be used internally by the internals of <see cref="IgsConnection"/>, 
    /// never directly by <see cref="IgsConnection"/>'s various user-facing methods. 
    /// </summary>
    internal class IgsRequest
    {

        /// <summary>
        /// Gets the command that should be sent to the server.
        /// </summary>
        public string Command { get; }
        public IgsRequest(string command) { Command = command; }

        /// <summary>
        /// As new lines arrive from the server, if the <see cref="IgsConnection"/> believes that they belong to this request,
        /// they are pushed to this buffer block using its Post extension method. This instance's <see cref="GetAllLines"/> method then
        /// takes lines from this block and serializes them into a simple list.   
        /// </summary>
        public readonly BufferBlock<IgsLine> IncomingLines = new BufferBlock<IgsLine>();

        /// <summary>
        /// An unattended request is enqueued in the outgoing queue just as attended requests. Then, when it's time, it's sent out,
        /// but immediately after it's successfully sent out, it loses its current-request status and the next request in the queue
        /// is sent out immediately. We do not except to receive any kind of reply from the server in response to this request.
        /// </summary>
        public bool Unattended;

        /// <summary>
        /// The returned task waits until the IGS SERVER sends all the response data to our command, terminated by a prompt line, and then
        /// it returns this data as a <see cref="List{T}"/>.
        /// </summary>
        public async Task<IgsResponse> GetAllLines()
        {
            IgsResponse lines = new IgsResponse();
            while (true)
            {
                IgsLine line = await IncomingLines.ReceiveAsync();
                if (ForgottenForever)
                {
                    throw new System.Exception("If this happens, we should examine this in detail.");
                }
                if (line.Code == IgsCode.Prompt)
                {
                    break;
                }
                lines.Add(line);
            }
            return lines;
        }

        private bool ForgottenForever;
        public void Disconnect()
        {
            ForgottenForever = true;
        }
    }
}