// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Stride.Core.Mathematics;

namespace Stride.Graphics;

/// <summary>Surface-to-display rotation; renderer folds into projection to skip engine compose.</summary>
internal enum SurfaceRotation
{
    Identity,
    Rotate90,
    Rotate180,
    Rotate270,
}

internal static class SurfaceRotationExtensions
{
    public static bool SwapsAxes(this SurfaceRotation rotation) => rotation is SurfaceRotation.Rotate90 or SurfaceRotation.Rotate270;

    /// <summary>Clip-space rotation per VK_SURFACE_TRANSFORM_*_BIT_KHR (row-major), multiplied after the projection.</summary>
    public static Matrix ToClipSpaceMatrix(this SurfaceRotation rotation) => rotation switch
    {
        SurfaceRotation.Rotate90 => new Matrix(
             0, -1, 0, 0,
             1,  0, 0, 0,
             0,  0, 1, 0,
             0,  0, 0, 1),
        SurfaceRotation.Rotate180 => new Matrix(
            -1,  0, 0, 0,
             0, -1, 0, 0,
             0,  0, 1, 0,
             0,  0, 0, 1),
        SurfaceRotation.Rotate270 => new Matrix(
             0,  1, 0, 0,
            -1,  0, 0, 0,
             0,  0, 1, 0,
             0,  0, 0, 1),
        _ => Matrix.Identity,
    };
}
