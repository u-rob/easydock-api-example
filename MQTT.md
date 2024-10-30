# easydock MQTT-API

Die API basiert auf JSON-Nachrichten, die über einen MQTT-Broker versendet
werden.

Folgende topics werden verwendet:

- easydock/api/requests – Diese Topic wird für API-Aufrufe verwendet
- easydock/api/reply – Diese Topic wird für Antworten der API verwendet

Jeder Aufruf muss die folgende Spezifikation erfüllen:

- message_id (string) – Für jede Nachricht einzigartige ID
- timestamp (long) – Aktuelle Zeit in Millisekunden
- method (string) – Die auszuführende Aktion
  - Verfügbare Aktionen:
    - start_mission 
- dock_sn (string) – SN des Docks, mit welchem die Aktion durchgeführt werden soll.
- data (JSON-Object) – Die Parameter für die Aktion

Jede Antwort erfüllt die folgende Spezifikation:
- message_id (string) – Einzigartige ID der Nachricht
- timestamp (number) – Zeitstempel, an welchem die Antwort verschickt wurde
- replying_to (string) – message_id der Aufrufnachricht
- status_code (number) – Numerischer Wert, welcher das Ergebnis darstellt
  - Mögliche Werte
    - 1 – Aufruf erfolgreich bearbeitet
    - 100 – Dock ist bereits beschäftigt
    - 255 – Interner Fehler
      - Ein interner Fehler kann durch eine ungültige Nachricht
        oder dadurch, dass EASYDOCK zur Zeit nicht für
        Annahme von API-Aufrufen verfügbar ist
- status_code_description (string) – Beschreibung des Status in Textform

## Aufrufe
### start_mission
Request:

````json
{
    "message_id": "c2b865b6-478c-4b9f-80fe-1bcd98b20b91",
    "timestamp": 1715778874224,
    "dock_sn": "7ABDE9081232190F",
    "method": "start_mission",
    "data": {
      "mission_name": "Inspektionsflug"
    }
}
````

Reply (Erfolg):

````json
{
    "message_id": "f09a1e70-5c56-45c0-ad77-879c6183cad1",
    "timestamp": 1715779016020,
    "replying_to": "c2b865b6-478c-4b9f-80fe-1bcd98b20b91",
    "status_code": 1,
    "status_code_description": "success"
}
````

Reply (Fehler):

````json
{
    "message_id": "a7fd55d4-9696-4d3e-bcd6-2a69029517c4",
    "timestamp": 1715779149528,
    "replying_to": "c2b865b6-478c-4b9f-80fe-1bcd98b20b91",
    "status_code": 100,
    "status_code_description": "Dock is already in operation"
}
````

### takeoff_to_point

Request:

````json
{
    "message_id": "c2b865b6-478c-4b9f-80fe-1bcd98b20b91",
    "timestamp": 1715778874224,
    "dock_sn": "7ABDE9081232190F",
    "method": "takeoff_to_point",
    "data": {
      "lat": 10.5,
      "lng": 20.123
    }
}
````

Reply (Erfolg):

````json
{
    "message_id": "f09a1e70-5c56-45c0-ad77-879c6183cad1",
    "timestamp": 1715779016020,
    "replying_to": "c2b865b6-478c-4b9f-80fe-1bcd98b20b91",
    "status_code": 1,
    "status_code_description": "success"
}
````

Reply (Fehler):

````json
{
    "message_id": "a7fd55d4-9696-4d3e-bcd6-2a69029517c4",
    "timestamp": 1715779149528,
    "replying_to": "c2b865b6-478c-4b9f-80fe-1bcd98b20b91",
    "status_code": 100,
    "status_code_description": "Dock is already in operation"
}
````
