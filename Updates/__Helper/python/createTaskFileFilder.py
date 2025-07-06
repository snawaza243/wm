import os
import calendar
from datetime import date

def create_year_structure(year):
    base_path = f"{year}"
    os.makedirs(base_path, exist_ok=True)

    for month in range(1, 13):
        month_name = calendar.month_name[month]
        month_folder = f"{base_path}/{month:02d}_{month_name}"
        os.makedirs(month_folder, exist_ok=True)

        for day in range(1, calendar.monthrange(year, month)[1] + 1):
            day_date = date(year, month, day)
            day_name = day_date.strftime("%A")
            file_name = f"{month_folder}/{day:02d}.txt"

            with open(file_name, "w") as file:
                file.write(f"{day_date.strftime('%Y-%m-%d')} - {day_name}")

    print(f"Folder structure for {year} created successfully!")

# Example usage
create_year_structure(2025)
