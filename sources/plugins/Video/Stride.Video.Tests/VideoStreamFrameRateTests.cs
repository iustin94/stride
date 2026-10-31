using System;
using FFmpeg.AutoGen;
using Stride.Video.FFmpeg;
using Xunit;

namespace Stride.Video.Tests;

public unsafe class VideoStreamFrameRateTests
{
    private static readonly AVRational Undeclared = new() { num = 0, den = 1 };

    [Fact]
    public void AStreamWhoseCodecDeclaresNoFrameRate_PlaysAtItsAverageFrameRate()
    {
        var fifteenPerSecond = new AVRational { num = 15, den = 1 };
        var parameters = new AVCodecParameters { codec_type = AVMediaType.AVMEDIA_TYPE_VIDEO, framerate = Undeclared };
        var stream = new AVStream { codecpar = &parameters, avg_frame_rate = fifteenPerSecond, r_frame_rate = fifteenPerSecond };

        var video = new VideoStream(&stream, null!);

        var oneFifteenthOfASecond = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / 15);
        Assert.Equal(15d, video.FPS);
        Assert.Equal(oneFifteenthOfASecond, video.FrameDuration);
    }

    [Fact]
    public void AStreamSlowerThanOneFramePerSecond_HoldsEachFrameForItsFullDuration()
    {
        var onePerTwoSeconds = new AVRational { num = 1, den = 2 };
        var parameters = new AVCodecParameters { codec_type = AVMediaType.AVMEDIA_TYPE_VIDEO, framerate = onePerTwoSeconds };
        var stream = new AVStream { codecpar = &parameters, avg_frame_rate = onePerTwoSeconds, r_frame_rate = onePerTwoSeconds };

        var video = new VideoStream(&stream, null!);

        var twoSeconds = TimeSpan.FromSeconds(2);
        Assert.Equal(0.5d, video.FPS);
        Assert.Equal(twoSeconds, video.FrameDuration);
    }

    [Fact]
    public void AStreamThatDeclaresNoFrameRateAnywhere_IsRefused()
    {
        var parameters = new AVCodecParameters { codec_type = AVMediaType.AVMEDIA_TYPE_VIDEO, framerate = Undeclared };
        var stream = new AVStream { codecpar = &parameters, avg_frame_rate = Undeclared, r_frame_rate = Undeclared };
        AVStream* rateless = &stream;

        Assert.Throws<UnknownVideoFrameRateException>(() => new VideoStream(rateless, null!));
    }
}
