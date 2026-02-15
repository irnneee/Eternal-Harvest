using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cambiar de escena o salir

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenuUI; // Referencia a todo el objeto del menú
    public static bool GameIsPaused = false;

    void Start()
    {
        // Esto asegura que el menú esté oculto al arrancar el juego
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
        
        // Aseguramos que el tiempo fluye normal al empezar
        Time.timeScale = 1f; 
        GameIsPaused = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false); // Ocultamos el menú
        Time.timeScale = 1f; // El tiempo vuelve a la normalidad
        GameIsPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true); // Mostramos el menú
        Time.timeScale = 0f; // Congelamos el tiempo del juego
        GameIsPaused = true;
    }

    // Función para el botón "Salir"
    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
    
    // Aquí se añadirá la lógica de Guardar más adelante
    public void SaveGame()
    {
        Debug.Log("Guardando...");
    }
}