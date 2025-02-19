import pandas as pd
import re

# Load the CSV file
file_path = "Species_Sightings_1km_Block_16Oct2024.csv"
df = pd.read_csv(file_path)

# Function to extract species, count, and date
def parse_species_data(entry):
    match = re.match(r"(.+) \((\d+)\) (\d{4}-\d{2}-\d{2})", entry)
    if match:
        species, count, date = match.groups()
        return species, int(count), date
    return None, None, None

# Apply parsing function to each row
df[['Species', 'Count', 'Date']] = df['Aggregated_Species_Data'].apply(
    lambda x: pd.Series(parse_species_data(x))
)

# Drop the original column
df.drop(columns=['Aggregated_Species_Data'], inplace=True)

# Save the structured data
df.to_csv("Structured_Bird_Data.csv", index=False)

print("Structured data saved as 'Structured_Bird_Data.csv'")
