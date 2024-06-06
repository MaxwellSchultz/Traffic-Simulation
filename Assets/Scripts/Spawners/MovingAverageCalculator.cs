using System.Collections.Generic;
using UnityEngine;

public sealed class MovingAverageCalculator
{
    private class TimeStampedValue
    {
        public float TimeStamp { get; }

        public TimeStampedValue(float timeStamp)
        {
            TimeStamp = timeStamp;
        }
    }
    private List<TimeStampedValue> values = new List<TimeStampedValue>();
    private float windowDuration = 60f; // 15 seconds

    public void AddValue()
    {

        float currentTime = Time.time;
        values.Add(new TimeStampedValue(currentTime));

        // Remove old values that fall out of the time window
        values.RemoveAll(v => currentTime - v.TimeStamp > windowDuration);
    }

    public float CalculateMovingAverage()
    {
        float currentTime = Time.time;
        int count = 0;

        foreach (var timeStampedValue in values)
        {
            if (currentTime - timeStampedValue.TimeStamp <= windowDuration)
            {
                count++;
            }
        }

        return count;
    }

    public void SetWindowDuration(float duration)
    {
        windowDuration = duration;
    }
}
