# Eternal Harvest

Eternal Harvest es un juego **en desarollo** 2D de simulación de granja y gestión del tiempo con temática mitológica desarrollado en Unity (URP).

## Sobre el Juego

Estás en el Tártaro y tu castigo es trabajar la tierra dentro de un invernadero. Encarnas a Francisco Javier, quien debe plantar y cosechar cultivos para alcanzar unas ganancias mínimas cada 10 días. Si no cumples con la cuota exigida, serás castigado por las Erinias: perderás el 25% de tu plantación (principalmente los cultivos de producción continua) y no recibirás el dinero acumulado en ese periodo.

En este lugar los días no duran 24 horas, sino que se rigen por un reloj de cansancio. Antes de que el medidor se llene por completo, tendrás que ir a dormir a tu choza en ruinas, donde Francisco Javier lleva la cuenta de los días en la pared (agrupados también en hecatómeros de 100 días). No puedes salir del invernadero hasta pagar tu tasa, donde además te espera Hermes con su tienda y los encargos divinos del Olimpo y el Elíseo.

## Características Implementadas

* **Movimiento y Animaciones 2D:** Control del personaje (Francisco Javier) en 4 direcciones con animaciones de caminar y reposo (`CharacterController2D.cs`), acompañado de seguimiento de cámara (`CameraFollow.cs`).
* **Sistema de Agricultura y Terreno:** Interacción con el mapa de tiles (`PlayerInteraction.cs`) utilizando reglas de terreno arado (`TilledRuleTile`) y suelo regado (`Tilemap_BienRegao`).
* **Ciclo de Cultivos:** Sistema de plantación, crecimiento por fases y recolección (`Plants.cs`) con dos cultivos implementados mediante ScriptableObjects y prefabs:
  * **Apó Páno:** Semillas, 4 fases de crecimiento y fruto cosechable.
  * **Avlavis:** Semillas, 4 fases de crecimiento y fruto cosechable.
* **Herramientas e Inventario:** Gestión de objetos recolectados y semillas (`Inventory.cs`, `ItemData.cs`) con interfaz gráfica (`InventoryUI.cs`) y menú selector para alternar entre Mano, Azada y Cubo/Regadera (`ToolMenuUI.cs`).
* **Reloj de Cansancio y Descanso:** Temporizador visual en el HUD con 6 fases de progresión (`Timer.cs`) y sistema para dormir e iniciar un nuevo día al interactuar con la choza (`HutSleepTrigger.cs`, `GameManager.cs`).
* **Interfaz y Pausa:** HUD integrado con barra de inventario, selector de herramientas y menú de pausa funcional (`PauseMenu.cs`).

## Controles Actuales

* **WASD / Flechas de dirección:** Mover al personaje
* **Clic Izquierdo / Interacción:** Usar herramienta seleccionada, arar, regar, plantar o cosechar
* **Q:** Abrir el menú de herramientas, con la rueda del ratón cambias la selección
* **Teclas del 1 al 0:** Selecciona el objeto del inventario principal
* **ESC:** Pausar el juego

## Estructura del Proyecto

* `Assets/Scenes/`: Contiene la escena principal jugable `Invernadero.unity`.
* `Assets/Scripts/`: Scripts de C# con la lógica del juego, controladores de animación (`FJ_Controller`) y clips de animación del personaje.
* `Assets/Items/` y `Assets/PreFabs/`: Datos (`ItemData`) y prefabs de las plantas y semillas (`ApoPano` y `Avlavis`), además del prefab `SueloArado`.
* `Assets/Images/`: Sprites del personaje, choza, árboles, fases de las plantas, tilemaps del invernadero y elementos del HUD (reloj, inventario, iconos de herramientas).
* `Assets/Fonts/`: Fuentes pixel art utilizadas en la interfaz (`BoldPixels` y `return-of-the-boss`).

## Instalación y Ejecución

1. Clona este repositorio en tu equipo:
   `git clone https://github.com/irnneee/eternal-harvest.git`
2. Abre **Unity Hub** y haz clic en **Add > Add project from disk**.
3. Selecciona la carpeta del proyecto clonado y ábrelo con tu versión de Unity compatible con URP 2D.
4. En la ventana Project, navega hasta `Assets/Scenes/` y abre la escena `Invernadero.unity`.
5. Pulsa el botón **Play** en la parte superior del editor para jugar.

## Autoras

* **Irene Rodríguez Morillo** - Parte técnica y programación (Scripts e integración en Unity)
* **Olga Rodríguez Morillo** - Parte visual y arte (Sprites, animaciones, interfaz y diseño de entorno)
