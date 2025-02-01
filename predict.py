import joblib
import sys
import pandas as pd
import json

# Podaj pełną ścieżkę do pliku JSON
json_path = r"C:\Users\48512\Documents\GitHub\ML---footbal_predictions\team_stats.json"

def predict_match(home_team, away_team):
    try:
        # Wczytaj dane drużyn z pliku JSON
        with open(json_path, 'r') as f:
            team_data = json.load(f)
    except Exception as e:
        return f"Error: Nie można wczytać danych drużyn: {e}"

    # Sprawdź, czy drużyny istnieją w bazie
    if home_team not in team_data:
        return f"Error: Drużyna gospodarza '{home_team}' nie istnieje w bazie."
    if away_team not in team_data:
        return f"Error: Drużyna gości '{away_team}' nie istnieje w bazie."

    # Tutaj dodaj logikę tworzenia new_match na podstawie danych z JSON
    new_match = {
        'FTHG': [team_data[home_team]['FTHG']],  # Użyj danych z JSON
        'FTAG': [team_data[away_team]['FTAG']],
        'HS': [team_data[home_team]['HS']],
        'AS': [team_data[away_team]['AS']],
        'HST': [team_data[home_team]['HST']],
        'AST': [team_data[away_team]['AST']],
        'HC': [team_data[home_team]['HC']],
        'AC': [team_data[away_team]['AC']],
        'HY': [team_data[home_team]['HY']],
        'AY': [team_data[away_team]['AY']],
        'HR': [team_data[home_team]['HR']],
        'AR': [team_data[away_team]['AR']],
        'GoalDiff': [team_data[home_team]['GoalDiff']]
    }

    # Wczytaj model i wykonaj predykcję
    model = joblib.load(r"C:\Users\48512\Documents\GitHub\ML---footbal_predictions\football_prediction_model.pkl")
    prediction = model.predict(pd.DataFrame(new_match))
    return prediction[0]

if __name__ == "__main__":
    try:
        if len(sys.argv) < 3:
            print("Error: Brakujące argumenty", file=sys.stderr)
            sys.exit(1)

        home_team = sys.argv[1]
        away_team = sys.argv[2]

        result = predict_match(home_team, away_team)

        if isinstance(result, str) and result.startswith("Error:"):
            print(result, file=sys.stderr)
            sys.exit(1)
        else:
            print(result)
    except Exception as e:
        print(f"Error: {e}", file=sys.stderr)
        sys.exit(1)