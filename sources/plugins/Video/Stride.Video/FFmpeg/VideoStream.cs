// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

#if STRIDE_VIDEO_FFMPEG
using System;
using FFmpeg.AutoGen;
using Stride.Core.Annotations;

namespace Stride.Video.FFmpeg
{
    /// <summary>
    /// Represents a video stream from a FFmpeg media.
    /// </summary>
    public sealed unsafe class VideoStream : FFmpegStream
    {
        public VideoStream([NotNull] AVStream* pStream, [NotNull] FFmpegMedia media)
            : base(pStream, media)
        {
            var pCodecpar = pStream->codecpar;
            AVRational frameRate = DeclaredFrameRate(pStream);
            long ticksPerFrame = TimeSpan.TicksPerSecond * frameRate.den / frameRate.num;
            FPS = (double)frameRate.num / frameRate.den;
            FrameDuration = TimeSpan.FromTicks(ticksPerFrame);
            PixelFormat = (AVPixelFormat)pCodecpar->format;
            Height = pCodecpar->height;
            Width = pCodecpar->width;
        }

        private static AVRational DeclaredFrameRate(AVStream* pStream)
        {
            AVRational[] declarations = [pStream->codecpar->framerate, pStream->avg_frame_rate, pStream->r_frame_rate];
            foreach (AVRational declaration in declarations)
            {
                if (IsAFrameRate(declaration))
                    return declaration;
            }

            throw new UnknownVideoFrameRateException(pStream->index);
        }

        private static bool IsAFrameRate(AVRational declaration)
        {
            bool CountsFrames() => declaration.num > 0;
            bool HasATimeUnit() => declaration.den > 0;
            return CountsFrames() && HasATimeUnit();
        }

        /// <summary>
        /// Video frames per second.
        /// </summary>
        public double FPS { get; }

        /// <summary>
        /// Time interval between two frames.
        /// </summary>
        /// <remarks>
        /// Is it equal to the inverse of <see cref="FPS"/>.
        /// </remarks>
        public TimeSpan FrameDuration { get; }

        /// <summary>
        /// The pixel format of the encoded video.
        /// </summary>
        public AVPixelFormat PixelFormat { get; }

        /// <inheritdoc />
        public override FFMpegStreamType Type => FFMpegStreamType.Video;

        /// <summary>
        /// The height of a frame in the video, in pixels.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// The width of a frame in the video, in pixels.
        /// </summary>
        public int Width { get; }
    }
}
#endif
