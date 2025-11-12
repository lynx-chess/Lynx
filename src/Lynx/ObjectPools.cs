using Lynx.Model;
using Microsoft.Extensions.ObjectPool;
using System.Text;

namespace Lynx;
public static class ObjectPools
{
    public static readonly ObjectPool<StringBuilder> StringBuilderPool =
        new DefaultObjectPoolProvider().CreateStringBuilderPool(
            initialCapacity: 256,
            maximumRetainedCapacity: 4 * 1024); //Default value in StringBuilderPooledObjectPolicy.MaximumRetainedCapacity)

    public static readonly ObjectPool<GameState> GameStatePool =
        new DefaultObjectPoolProvider().Create(new GameStatePooledObjectPolicy());
}

public class GameStatePooledObjectPolicy : PooledObjectPolicy<GameState>
{
    public override GameState Create() => new();

    public override bool Return(GameState _) => true;
}
