import pandas as pd
import sqlite3

df = pd.read_csv('Structured_Bird_Data.csv')

conn = sqlite3.connect('BirdSightings.db')
df.to_sql('BirdObservations', conn, if_exists='append', index=False)
conn.close()