// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

#if STRIDE_VIDEO_FFMPEG
using System;

namespace Stride.Video.FFmpeg
{
    public sealed class UnknownVideoFrameRateException : InvalidOperationException
    {
        public UnknownVideoFrameRateException(int streamIndex)
            : base($"Video stream {streamIndex} declares no frame rate: its codec, average and real frame rates are all unknown.")
        {
        }
    }
}
#endif
