import os
import calendar
from datetime import datetime

def create_year_structure(year, base_path="."):
    # Base directory for the given year
    year_path = os.path.join(base_path, str(year))
    os.makedirs(year_path, exist_ok=True)
    
    for month in range(1, 13):  # Loop through all 12 months
        month_name = calendar.month_name[month]  # Get month name
        month_path = os.path.join(year_path, month_name)
        os.makedirs(month_path, exist_ok=True)  # Create month directory
        
        _, num_days = calendar.monthrange(year, month)  # Get total days in the month
        
        for day in range(1, num_days + 1):  # Loop through each day
            date_str = f"{year}-{month:02d}-{day:02d}"
            day_name = datetime(year, month, day).strftime("%A")
            file_name = f"{day:02d}.txt"  # Filename format
            file_path = os.path.join(month_path, file_name)
            
            # Write date and day name in the file
            with open(file_path, "w") as file:
                file.write(f"{date_str} - {day_name}\n")
    
    print(f"Folders and files created successfully for {year}")

# Example usage
year = 2025  # Replace with the desired year
create_year_structure(year)
