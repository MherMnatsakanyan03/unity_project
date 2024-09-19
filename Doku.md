# Arbeitsprotokoll

Autoren: Mher Mnatsakanyan,

Der Aufwand wurde gleichmäßig aufgeteilt, wobei jeder von uns einen ähnlichen Gesamtaufwand in der Umsetzung des Projekts hatte. Die Aufgaben wurden in der Gruppe besprochen und gemeinsam umgesetzt. Die Aufgabenverteilung war dabei nicht strikt festgelegt, sondern wurde je nach Bedarf und Interesse der Gruppenmitglieder angepasst.

## Unity

Die größte Herausforderung für uns alle war es zu lernen, wie man mit Unity arbeitet. Wir können behaupten, dass wir viel aus dem Projekt gelernt haben und wenn wir das Modul nochmal wiederholen würden, dann könnten wir vieles deutlich einfacher und schneller umsetzen.

Trotzdem hatten wir einige Probleme, die uns die Arbeit erschwert haben. Ein Problem war die Performance der Engine, die für lange Wartezeiten sorgte. Ein weiteres Problem war die komplexe Dokumentation. Zwar gibt es Online viele Foren und Tutorials, aber es war recht schwer sich zurecht zu finden, zumal due Engine sehr komplex ist und viele Funktionen bietet. So war es nicht immer einfach die passende Lösung aus vielen verschiedenen Möglichkeiten zu finden.

## Preprocessing

Die Idee hinter dem Preprocesisng war es die Daten aus der CSV Datei in eine einfach zu handhabende Datenstruktur zu überführen.

Grundsätzlich war die Überlegung eine Art Heap für die Daten zu erstellen, in dem die Daten in einer Baumstruktur abgelegt werden. Die Baumstruktur sollte dabei so aufgebaut sein, dass die Daten in der hierarchischen Struktur der Repräsentation der Stadt entsprechen. D.h. zuerst haben wir uns überlegt, wie die Stadt und die Steuerung grundsätzlich aufgebaut sind und wie wir diese Struktur in einer Baumstruktur abbilden können. So haben wir die Daten in folgender Reihenfolge strukturiert:

Jahr -> Stadt -> Gebäudeklasse -> Facilities -> Gebäude

Im Prinzip hat jedes Jahr mehrere Städte, jede Stadt mehrere Gebäudeklassen, jede Gebäudeklasse mehrere Facilities und jede Facility mehrere Gebäude.
Mit dieser Überlegung haben wir das Preprocessing implementiert, wobei wir Zeile für Zeile durch die CSV Datei iteriert haben und die Daten in die entsprechende Struktur abgelegt haben. Die Überlegung da war falls aus der ersten Hierarchieebene ein Wert fehlt, wird die Zeile als komplett neues Element angelegt. Falls es bereits ein Element aus dieser Hierarchieebene gibt, so wird der Wert der nächsten Hierarchieebene sich angeschaut und die Schritte werden wiederholt. So garantiert man, dass am Ende die Daten in der richtigen Struktur abgelegt sind.

Einige Probleme bei der Entwicklung waren:

- die Schleife musste zum Teil mehrfach durchlaufen werden, da wir Durschnittswerte für gewisse Kategorien berechnen mussten
- es gab in der Entwicklungsphase neue Erweiterungswünsche, weshalb die Struktur des Baumes mehrmals angepasst werden musste
- wir haben recht simpel versucht die Liste zu sortieren, aber aus einem uns unerklärlichen Grund hat dann die Stadtgenerierung nicht funktioniert

## Kamerasteuerung

In der ersten überlegung wollten wir eine einfache steuerung haben, wobei man als User nicht viel kontrolle hat, sondern die Kamera eine Stadt fokussiert und man nur durch das wechseln von Stadt zu Stadt die Kamera bewegen kann. Das hat sich aber als nicht so gut herausgestellt, da wir in der Prototypphase der Stadtgeneration festgestellt haben, dass die Städte nicht immer gleich groß sind und manche Städte sehr klein sind, andere wiederum sehr groß. Das hat dazu geführt, dass in großen Städten die Gebäude winzig waren, um was zu erkennen. Aus diesem Grund haben wir uns entschieden eine freie Kamerasteuerung zu implementieren. Zwar blieb das Prinzip der Stadtfokussierung erhalten, aber innerhalb einer Stadt kann man nun mit den Pfeiltasten die Kamera bewegen und mit Shift und Strg den Zoom steuern.

Probleme bei der Umsetzung waren:

- die Städte wurde in einem Schachbrettmuster generiert, was dazu führte, was dazu führte, dass die Zentrierung der Kamera durch die unterschiedlichen Größen der Städte nicht so einfach wie in der Theorie war
- die Kamera sollte sich flüssig bewegen, was aber durch die Performance der Engine erschwert wurde
- eine flüssige Bewegung, d.h. auch eine gute Wahl der Geschwindigkeit der Kamera, war schwer zu finden, vor allem abhängig vom Zoomlevel
