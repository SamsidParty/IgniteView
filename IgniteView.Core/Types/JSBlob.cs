using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.Core.Types
{
    public class JSBlob : IDisposable
    {
        private static ConcurrentDictionary<string, TaskCompletionSource<JSBlob>> BlobResolutionQueue = new();

        public string ID;
        public Stream Stream;
        public TaskCompletionSource ReadCompletionSource = new TaskCompletionSource();

        /// <summary>
        /// Waits until the JavaScript uploads the blob with the given ID, then returns it.
        /// </summary>
        public static async Task<JSBlob> WaitForBlobResolution(string blobID)
        {
            TaskCompletionSource<JSBlob> completionSource;

            if (!BlobResolutionQueue.ContainsKey(blobID))
            {
                BlobResolutionQueue[blobID] = new TaskCompletionSource<JSBlob>();
                await BlobResolutionQueue[blobID].Task.WaitAsync(CancellationToken.None); // Wait for the blob to be uploaded
            }

            completionSource = BlobResolutionQueue[blobID];
            var result = completionSource.Task.Result;

            return result;
        }

        public JSBlob(string id, Stream stream)
        {
            ID = id;
            Stream = stream;

            lock (BlobResolutionQueue)
            {
                if (BlobResolutionQueue.ContainsKey(ID))
                { // If WaitForBlobResolution has already been called
                    var existingCompletionSource = BlobResolutionQueue[ID];
                    existingCompletionSource.SetResult(this);
                }
                else // WaitForBlobResolution hasn't been called yet
                {
                    BlobResolutionQueue[ID] = new TaskCompletionSource<JSBlob>();
                    BlobResolutionQueue[ID].SetResult(this);
                }
            }
        }

        public void Dispose() { 
            ReadCompletionSource.SetResult();
            BlobResolutionQueue.TryRemove(ID, out _);
        }
    }
}
