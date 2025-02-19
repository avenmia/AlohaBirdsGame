import pandas as pd
import re

# Load the CSV file
file_path = "Species_Sightings_1km_Block_16Oct2024.csv"
df = pd.read_csv(file_path)

# Function to extract species, count, and date
def parse_species_data(entry):
    observations = entry.split(", ")
    parsed_entries = []
    for obs in observations:
        match = re.match(r"(.+) \((\d+)\) (\d{4}-\d{2}-\d{2})", obs)
        if match:
            species, count, date = match.groups()
            parsed_entries.append((species, int(count), date))
    return parsed_entries

# Expand rows for multiple species observations
expanded_rows = []
for _, row in df.iterrows():
    parsed_entries = parse_species_data(row['Aggregated_Species_Data'])
    for species, count, date in parsed_entries:
        expanded_rows.append({
            'Latitude': row['Latitude'],
            'Longitude': row['Longitude'],
            'Species': species,
            'Count': count,
            'Date': date
        })

# Create a new DataFrame with structured data
structured_df = pd.DataFrame(expanded_rows)

# Save the structured data
structured_df.to_csv("Structured_Bird_Data.csv", index=False)

print("Structured data saved as 'Structured_Bird_Data.csv'")