using UnityEngine;

// Define an interface for reacting to being hit by the raycast
public interface IsHitReaction
{
    void ReactToHit();
    void UnreactToHit();
}
