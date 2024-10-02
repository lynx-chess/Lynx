﻿using Microsoft.Extensions.ObjectPool;
using System.Text;

namespace Lynx;
public static class ObjectPools
{
    public static readonly ObjectPool<StringBuilder> StringBuilderPool =
        new DefaultObjectPoolProvider().CreateStringBuilderPool(
            initialCapacity: 256,
            maximumRetainedCapacity: 4 * 1024); //Default value in StringBuilderPooledObjectPolicy.MaximumRetainedCapacity)
}
