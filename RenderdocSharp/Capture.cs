using System;

namespace RenderdocSharp
{
    public readonly record struct Capture(int Index, string FileName, DateTime Timestamp)
    {
        public void SetComments(string comments)
        {
            Renderdoc.SetCaptureFileComments(FileName, comments);
        }
    }
}