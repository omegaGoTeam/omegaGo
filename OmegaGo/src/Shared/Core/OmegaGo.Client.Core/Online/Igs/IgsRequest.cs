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
        public IgsRequest(string command) { this.Command = command; }

        /// <summary>
        /// As new lines arrive from the server, if the <see cref="IgsConnection"/> believes that they belong to this request,
        /// they are pushed to this buffer block using its Post extension method. This instance's <see cref="GetAllLines"/> method then
        /// takes lines from this block and serializes them into a simple list.   
        /// </summary>
        public BufferBlock<IgsLine> IncomingLines = new BufferBlock<IgsLine>();

        public bool Unattended;

        /// <summary>
        /// The returned task waits until the IGS SERVER sends all the response data to our command, terminated by a prompt line, and then
        /// it returns this data as a <see cref="List{T}"/>.
        /// </summary>
        public async Task<List<IgsLine>> GetAllLines()
        {
            List<IgsLine> lines = new List<IgsLine>();
            while (true)
            {
                IgsLine line = await this.IncomingLines.ReceiveAsync();
                if (line.Code == IgsCode.Prompt)
                {
                    break;
                }
                lines.Add(line);
            }
            return lines;
        }
    }
}