import pandas as pd
from sklearn.ensemble import RandomForestClassifier
from sklearn.metrics import precision_score, classification_report
from sklearn.model_selection import cross_val_score

# Wczytanie danych
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

# Uporządkowanie kolumn i połączenie danych
common_cols = sorted(list(cols1.union(cols2)))
matches = matches[common_cols]
matches2 = matches2[common_cols]

combined_data = pd.concat([matches, matches2], ignore_index=True)

# Przetwarzanie danych
combined_data["Date"] = pd.to_datetime(combined_data["Date"], dayfirst=True)
combined_data["venue_code"] = combined_data["HomeTeam"].astype("category").cat.codes
combined_data["opp_code"] = combined_data["AwayTeam"].astype("category").cat.codes
combined_data["hour"] = combined_data["Time"].str.replace(":.+", "", regex=True).astype("int")
combined_data["day_code"] = combined_data["Date"].dt.day_of_week
combined_data["target"] = combined_data["FTR"].map({"H": 1, "D": 0, "A": -1})

# Obliczanie średnich kroczących
cols = ["FTHG", "FTAG", "HS", "HST"]
new_cols = [f"{c}_rolling" for c in cols]

def rolling_averages(group, cols, new_cols):
    group = group.sort_values("Date")
    rolling_stats = group[cols].rolling(3, closed="left").mean()
    group[new_cols] = rolling_stats
    return group.dropna()

matches_rolling = combined_data.groupby("HomeTeam", group_keys=False).apply(
    lambda x: rolling_averages(x, cols, new_cols)
)

# Predykcja
def make_prediction(data, predictors):
    # Podział na zbiór treningowy i testowy
    train = data[data["Date"] < "2024-01-01"]
    test = data[data["Date"] >= "2024-01-01"]

    # Trenowanie modelu
    rf = RandomForestClassifier(n_estimators=50, min_samples_split=10, random_state=1)
    rf.fit(train[predictors], train["target"])

    # Predykcja na zbiorze testowym
    preds = rf.predict(test[predictors])

    # Ocena modelu
    precision = precision_score(test["target"], preds, average="weighted", zero_division=0)
    report = classification_report(test["target"], preds, target_names=["Away Win", "Draw", "Home Win"], zero_division=0)

    return preds, precision, report, test

# Walidacja krzyżowa
def cross_validate_model(data, predictors):
    rf = RandomForestClassifier(n_estimators=50, min_samples_split=10, random_state=1)
    scores = cross_val_score(rf, data[predictors], data["target"], cv=5, scoring="precision_weighted")
    return scores.mean()

# Użycie modelu
predictors = ["venue_code", "opp_code", "hour", "day_code", "FTHG_rolling", "FTAG_rolling"]

# Predykcja
results, precision, report, test_data = make_prediction(matches_rolling, predictors)

# Walidacja krzyżowa
cv_precision = cross_validate_model(matches_rolling, predictors)

# Zapis wyników
test_data["predicted_result"] = results
test_data.to_csv("predictions.csv", index=False)

# Wyświetlenie wyników
print(f"Precision on test set: {precision}")
print(f"Cross-validation precision: {cv_precision}")
print("Classification Report:")
print(report)
print("Predictions saved to 'predictions.csv'")