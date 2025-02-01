import pandas as pd
import numpy as np
from sklearn.model_selection import train_test_split
from sklearn.ensemble import RandomForestClassifier
from sklearn.metrics import accuracy_score
import joblib

# 1. Wczytanie danych
df_e0 = pd.read_csv(r'C:\Users\48512\Documents\GitHub\ML---footbal_predictions\Data\E0.csv')
df_e1 = pd.read_csv(r'C:\Users\48512\Documents\GitHub\ML---footbal_predictions\Data\E1.csv')

# Sprawdź, czy dane zostały wczytane
print(df_e0.head())
print(df_e1.head())

# 2. Połączenie danych
df = pd.concat([df_e0, df_e1], ignore_index=True)

# 3. Czyszczenie danych
# Przekonwertuj kolumny numeryczne
numeric_columns = ['FTHG', 'FTAG', 'HS', 'AS', 'HST', 'AST', 'HC', 'AC', 'HY', 'AY', 'HR', 'AR']
for col in numeric_columns:
    df[col] = pd.to_numeric(df[col], errors='coerce')

# Wypełnij brakujące wartości medianą tylko dla kolumn numerycznych
df.fillna(df.median(numeric_only=True), inplace=True)

# 4. Feature engineering
df['GoalDiff'] = df['FTHG'] - df['FTAG']
df['Result'] = np.where(df['FTHG'] > df['FTAG'], 1, np.where(df['FTHG'] == df['FTAG'], 0, -1))

# 5. Przygotowanie danych do modelowania
features = ['FTHG', 'FTAG', 'HS', 'AS', 'HST', 'AST', 'HC', 'AC', 'HY', 'AY', 'HR', 'AR', 'GoalDiff']
X = df[features]
y = df['Result']

# 6. Podział danych na zbiór treningowy i testowy
X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42)

# 7. Trenowanie modelu
model = RandomForestClassifier(n_estimators=100, random_state=42)
model.fit(X_train, y_train)

# 8. Ocena modelu
y_pred = model.predict(X_test)
print(f'Dokładność modelu: {accuracy_score(y_test, y_pred) * 100:.2f}%')

# 9. Eksport modelu
joblib.dump(model, 'football_prediction_model.pkl')

# 10. Przykład użycia modelu
new_match = pd.DataFrame({
    'FTHG': [2],  # Liczba goli strzelonych przez gospodarzy
    'FTAG': [1],  # Liczba goli strzelonych przez gości
    'HS': [10],   # Strzały gospodarzy
    'AS': [5],    # Strzały gości
    'HST': [4],   # Strzały celne gospodarzy
    'AST': [2],   # Strzały celne gości
    'HC': [6],    # Corner kicks gospodarzy
    'AC': [3],    # Corner kicks gości
    'HY': [2],    # Żółte kartki gospodarzy
    'AY': [1],    # Żółte kartki gości
    'HR': [0],    # Czerwone kartki gospodarzy
    'AR': [0],    # Czerwone kartki gości
    'GoalDiff': [1]  # Różnica goli
})

prediction = model.predict(new_match)
print(f'Przewidywany wynik: {prediction[0]}')  # 1 = wygrana gospodarzy, 0 = remis, -1 = wygrana gości