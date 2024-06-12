import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sns

# Read the CSV file
df = pd.read_csv('simplestopsink.csv')

# Convert the time column to a numeric format
# Assuming the time format is minutes:seconds.milliseconds

firstrun = True
minNumSecs = 0

def convert_time_to_seconds(time_str):
    global firstrun, minNumSecs
    try:
        hours, minutes, seconds = time_str.split(':')
        total_seconds = int(hours) * 3600 + int(minutes) * 60 + float(seconds)
        if firstrun:
            minNumSecs = total_seconds
            firstrun = False
        
        return total_seconds - minNumSecs
    except ValueError as e:
        print(f"Error converting time string '{time_str}': {e}")
        return None
df['time_in_seconds'] = df['time'].apply(convert_time_to_seconds)



# Plot the data using seaborn
plt.figure(figsize=(10, 6))
sns.scatterplot(data=df, x='time_in_seconds', y='y1', marker='o', label='Num Spawned')
# sns.scatterplot(data=df, x='time_in_seconds', y='y2', marker='o', label='Avg Car Lifetime')

# sns.regplot(data=df, x='time_in_seconds', y='y2', scatter=False, label='Avg Lifetime Fit')

plt.xlabel('Time (seconds)')
plt.ylabel('Average Car Lifetime')
plt.title('Plot of Simple Stopsign')
plt.legend()
plt.grid(True)
plt.show()