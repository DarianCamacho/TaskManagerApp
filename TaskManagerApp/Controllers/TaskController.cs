using Firebase.Storage;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using TaskManagerApp.Models;
using WebAppCondominio.FirebaseAuth;

namespace TaskManagerApp.Controllers
{
    public class TaskController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult List()
        {
            return GetTasks();
        }

        private IActionResult GetTasks()
        {
            TasksHandler tasksHandler = new TasksHandler();

            ViewBag.Tasks = tasksHandler.GetTasksCollection().Result;

            return View("List");
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(string title, string description, string duedate, string iscompleted, string priority)
        {
            try
            {
                // Parsear la fecha a un objeto DateTime
                if (!DateTime.TryParse(duedate, out DateTime dueDate))
                {
                    ModelState.AddModelError("DueDate", "Fecha de vencimiento no válida");
                    return View(); // Puedes retornar la vista con el modelo para mostrar el mensaje de error.
                }

                // Verificar si la fecha de vencimiento está en el pasado
                if (dueDate < DateTime.Now.Date)
                {
                    ModelState.AddModelError("DueDate", "La fecha de vencimiento no puede estar en el pasado.");
                    return View(); // Puedes retornar la vista con el modelo para mostrar el mensaje de error.
                }

                // Si la fecha pasa las validaciones, continuar con la lógica de creación
                TasksHandler tasksHandler = new TasksHandler();
                bool result = tasksHandler.Create(title, description, dueDate.ToString(), iscompleted, priority).Result;

                return GetTasks();
            }
            catch (FirebaseStorageException ex)
            {
                ViewBag.Error = new ErrorHandler()
                {
                    Title = ex.Message,
                    ErrorMessage = ex.InnerException?.Message,
                    ActionMessage = "Go back",
                    Path = "/Task"
                };

                return View("ErrorHandler");
            }
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditTask(string id, string title, string description, string duedate, string iscompleted, string priority)
        {
            try
            {
                TasksHandler tasksHandler = new TasksHandler();

                bool result = tasksHandler.Edit(id, title, description, duedate, iscompleted, priority).Result;

                return GetTasks();
            }

            catch (FirebaseStorageException ex)
            {
                ViewBag.Error = new ErrorHandler()
                {
                    Title = ex.Message,
                    ErrorMessage = ex.InnerException?.Message,
                    ActionMessage = "Go to Home",
                    Path = "/Task"
                };

                return View("ErrorHandler");
            }
        }

        //Edit
        public IActionResult Edit(string id, string title, string description, string duedate, string iscompleted, string priority)
        {
            try
            {
                // Parsear la fecha a un objeto DateTime
                if (!DateTime.TryParse(duedate, out DateTime dueDate))
                {
                    ModelState.AddModelError("DueDate", "Fecha de vencimiento no válida");
                    ViewBag.Edited = new Tarea
                    {
                        Id = id,
                        Title = title,
                        Description = description,
                        DueDate = duedate,
                        IsCompleted = iscompleted,
                        Priority = priority
                    };

                    return View();
                }

                // Verificar si la fecha de vencimiento está en el pasado
                if (dueDate < DateTime.Now.Date)
                {
                    ModelState.AddModelError("DueDate", "La fecha de vencimiento no puede estar en el pasado.");
                    ViewBag.Edited = new Tarea
                    {
                        Id = id,
                        Title = title,
                        Description = description,
                        DueDate = duedate,
                        IsCompleted = iscompleted,
                        Priority = priority
                    };

                    return View();
                }

                ViewBag.Edited = new Tarea
                {
                    Id = id,
                    Title = title,
                    Description = description,
                    DueDate = dueDate.ToString(),
                    IsCompleted = iscompleted,
                    Priority = priority
                };

                return View();
            }
            catch (FirebaseStorageException ex)
            {
                ViewBag.Error = new ErrorHandler()
                {
                    Title = ex.Message,
                    ErrorMessage = ex.InnerException?.Message,
                    ActionMessage = "Go to Home",
                    Path = "/Task"
                };

                return View("ErrorHandler");
            }
        }

        [HttpPost]
        public IActionResult UpdateTaskStatus(string id, bool isCompleted)
        {
            try
            {
                TasksHandler tasksHandler = new TasksHandler();

                // Obtener la tarea existente
                Tarea existingTask = tasksHandler.GetTaskById(id).Result;

                if (existingTask == null)
                {
                    return Json(new { success = false, error = "Tarea no encontrada." });
                }

                // Actualizar el estado de la tarea
                existingTask.IsCompleted = isCompleted ? "true" : "false"; // Convertir el valor booleano a string

                // Guardar la tarea actualizada en la base de datos
                bool result = tasksHandler.Edit(existingTask.Id, existingTask.Title, existingTask.Description, existingTask.DueDate, existingTask.IsCompleted, existingTask.Priority).Result;

                if (result)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, error = "Error al actualizar la tarea." });
                }
            }
            catch (FirebaseStorageException ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string TaskId)
        {
            try
            {
                // Primero, obtén la referencia al documento de la tarjeta que deseas eliminar en Firebase
                var cardDocRef = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId)
                    .Collection("Tasks")
                    .Document(TaskId);

                // Borra el documento de la tarjeta
                await cardDocRef.DeleteAsync();

                // Redirige a la vista principal (Index) después de eliminar la tarjeta
                return RedirectToAction("List", "Task");
            }
            catch (Exception ex)
            {
                // Manejar errores
                Console.WriteLine("Error al eliminar la tarea: " + ex.Message);
                return View();
            }
        }
    }
}
