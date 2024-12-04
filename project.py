import pandas as pd
from sklearn.ensemble import RandomForestClassifier
from sklearn.metrics import accuracy_score, precision_score, classification_report

# Wczytanie danych
matches = pd.read_csv("E0.csv", index_col=0)

# Wyświetlanie kilku pierwszych wierszy oraz kształtu danych
print(matches.head())
print(matches.shape)

# Przekształcenie daty, aby nie pojawiało się ostrzeżenie
matches["Date"] = pd.to_datetime(matches["Date"], dayfirst=True)

# Dodanie kodów dla drużyn
matches["venue_code"] = matches["HomeTeam"].astype("category").cat.codes
matches["opp_code"] = matches["AwayTeam"].astype("category").cat.codes

# Dodanie godziny meczu
matches["hour"] = matches["Time"].str.replace(":.+", "", regex=True).astype("int")

# Dodanie kodu dnia tygodnia
matches["day_code"] = matches["Date"].dt.day_of_week

# Tworzenie kolumny 'target' z wynikiem meczu (1 - wygrana gospodarzy, 0 - remis, -1 - wygrana gości)
matches["target"] = matches["FTR"].map({"H": 1, "D": 0, "A": -1})

# Sprawdzenie brakujących danych
print("Brakujące dane:\n", matches.isnull().sum())

# Tworzenie modelu Random Forest
rf = RandomForestClassifier(n_estimators=50, min_samples_split=10, random_state=1)

# Podział na zbiory treningowe i testowe
train = matches[matches["Date"] < '2024-09-30']
test = matches[matches["Date"] >= '2024-09-30']

# Predyktory używane do trenowania modelu
predictor = ["venue_code", "opp_code", "hour", "day_code"]

# Trenowanie modelu
rf.fit(train[predictor], train["target"])

# Predykcje na zbiorze testowym
preds = rf.predict(test[predictor])

# Ocena modelu
acc = accuracy_score(test["target"], preds)
print(f"Dokładność modelu: {acc}")

# Tabela porównująca predykcje z rzeczywistością
combined = pd.DataFrame(dict(actual=test["target"], prediction=preds))
print(pd.crosstab(index=combined["actual"], columns=combined["prediction"]))

# Ocena precision dla wieloklasowego problemu
# Ustawienie `average='weighted'` dla obsługi wieloklasowej
prec = precision_score(test["target"], preds, average='weighted')
print(f"Precision (wieloklasowe): {prec}")

# Szczegółowy raport klasyfikacji
print("\nRaport klasyfikacji:\n")
print(classification_report(test["target"], preds))
