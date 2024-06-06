using System.Collections.Generic;
using UnityEngine;

public sealed class MovingAverageCalculator
{
    private class TimeStampedValue
    {
        public float TimeStamp { get; }
        public float Value { get; }

        public TimeStampedValue(float timeStamp, float value)
        {
            TimeStamp = timeStamp;
            Value = value;
        }
    }
    private List<TimeStampedValue> values = new List<TimeStampedValue>();
    private float windowDuration = 60f; // 15 seconds

    public void AddValue(float value)
    {
        float currentTime = Time.time;
        values.Add(new TimeStampedValue(currentTime, value));

        // Remove old values that fall out of the time window
        values.RemoveAll(v => currentTime - v.TimeStamp > windowDuration);
    }

    public float CalculateMovingAverage(float currentTime)
    {
        float sum = 0f;
        int count = 0;

        foreach (var timeStampedValue in values)
        {
            if (currentTime - timeStampedValue.TimeStamp <= windowDuration)
            {
                sum += timeStampedValue.Value;
                count++;
            }
        }

        return count > 0 ? sum / count : 0f;
    }

    public float CalculateMovingSum(float currentTime)
    {
        float sum = 0f;
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
