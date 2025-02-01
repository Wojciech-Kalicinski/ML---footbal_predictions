import joblib
import sys
import pandas as pd

def predict_match(home_team, away_team):
    # Wczytaj model
    model = joblib.load('football_prediction_model.pkl')
    
    # Przykładowe dane wejściowe (muszą być zgodne z formatem użytym podczas trenowania)
    new_match = {
        'FTHG': [2],  # Przykładowa liczba goli strzelonych przez gospodarzy
        'FTAG': [1],  # Przykładowa liczba goli strzelonych przez gości
        'HS': [10],   # Przykładowa liczba strzałów gospodarzy
        'AS': [5],    # Przykładowa liczba strzałów gości
        'HST': [4],   # Przykładowa liczba strzałów celnych gospodarzy
        'AST': [2],   # Przykładowa liczba strzałów celnych gości
        'HC': [6],    # Przykładowa liczba rzutów rożnych gospodarzy
        'AC': [3],    # Przykładowa liczba rzutów rożnych gości
        'HY': [2],    # Przykładowa liczba żółtych kartek gospodarzy
        'AY': [1],    # Przykładowa liczba żółtych kartek gości
        'HR': [0],    # Przykładowa liczba czerwonych kartek gospodarzy
        'AR': [0],    # Przykładowa liczba czerwonych kartek gości
        'GoalDiff': [1]  # Przykładowa różnica goli
    }
    
    # Przewidywanie wyniku
    prediction = model.predict(pd.DataFrame(new_match))
    return prediction[0]  # 1 = wygrana gospodarzy, 0 = remis, -1 = wygrana gości

if __name__ == "__main__":
    try:
        # Sprawdź, czy przekazano wymagane argumenty
        if len(sys.argv) < 3:
            print("Error: Missing arguments. Usage: python predict.py <home_team> <away_team>")
        else:
            home_team = sys.argv[1]
            away_team = sys.argv[2]
            print(f"Predicting match: {home_team} vs {away_team}")
            result = predict_match(home_team, away_team)
            print(result)
    except Exception as e:
        print(f"Error: {e}")