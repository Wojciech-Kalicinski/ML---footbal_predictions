import pandas as pd
from sklearn.ensemble import RandomForestClassifier
from sklearn.metrics import precision_score

# Wczytanie plików, pomijając problematyczne wiersze
matches = pd.read_csv("E0.csv", index_col=0, on_bad_lines="skip")
matches2 = pd.read_csv("E1.csv", index_col=0, on_bad_lines="skip")

# Dopasowanie brakujących kolumn
cols1 = set(matches.columns)
cols2 = set(matches2.columns)

missing_in_matches = cols2 - cols1
missing_in_matches2 = cols1 - cols2

for col in missing_in_matches:
    matches[col] = None
for col in missing_in_matches2:
    matches2[col] = None

# Uporządkowanie kolumn
common_cols = sorted(list(cols1.union(cols2)))
matches = matches[common_cols]
matches2 = matches2[common_cols]

# Połączenie danych
combined_data = pd.concat([matches, matches2], ignore_index=True)

# Przetwarzanie
combined_data["Date"] = pd.to_datetime(combined_data["Date"], dayfirst=True)
combined_data["venue_code"] = combined_data["HomeTeam"].astype("category").cat.codes
combined_data["opp_code"] = combined_data["AwayTeam"].astype("category").cat.codes
combined_data["hour"] = combined_data["Time"].str.replace(":.+", "", regex=True).astype("int")
combined_data["day_code"] = combined_data["Date"].dt.day_of_week
combined_data["target"] = combined_data["FTR"].map({"H": 1, "D": 0, "A": -1})

# Rolling averages
cols = ["FTHG", "FTAG", "HS", "HST"]
new_cols = [f"{c}_rolling" for c in cols]

def rolling_averages(group, cols, new_cols):
    group = group.sort_values("Date")
    rolling_stats = group[cols].rolling(3, closed="left").mean()
    for new_col, col in zip(new_cols, cols):
        group[new_col] = rolling_stats[col]
    return group.dropna()

matches_rolling = combined_data.groupby("HomeTeam", group_keys=False).apply(
    lambda x: rolling_averages(x, cols, new_cols)

    
)

# Predykcja
def make_prediction(data, predictors):
    train = data[data["Date"] < "2024-01-01"]
    test = data[data["Date"] >= "2024-01-01"]
    rf = RandomForestClassifier(n_estimators=50, min_samples_split=10, random_state=1)
    rf.fit(train[predictors], train["target"])
    preds = rf.predict(test[predictors])
    precision = precision_score(test["target"], preds, average="weighted", zero_division=1)
    return preds, precision

predictors = ["venue_code", "opp_code", "hour", "day_code", "FTHG_rolling", "FTAG_rolling"]
results, precision = make_prediction(matches_rolling, predictors)

print(f"Precision: {precision}")
 
print(matches_rolling)