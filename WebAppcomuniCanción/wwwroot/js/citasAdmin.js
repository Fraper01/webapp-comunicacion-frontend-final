document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.status-dropdown').forEach(function (dropdown) {
        dropdown.addEventListener('change', function () {
            var citaId = this.dataset.citaId;
            var newStatus = this.value;

            if (newStatus === "") {
                return; // No hacer nada si selecciona la opción por defecto
            }

            Swal.fire({
                title: '¿Estás seguro?',
                text: `¿Quieres cambiar el estatus de la cita ${citaId} a "${newStatus}"?`,
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Sí, cambiar',
                cancelButtonText: 'Cancelar'
            }).then((result) => {
                if (result.isConfirmed) {
                    // *** CAMBIOS APLICADOS AQUÍ ***
                    fetch(`http://localhost:5000/api/Citas/${citaId}/estatus`, { // Correcta URL: /api/Citas/{id}/estatus en el puerto 5000
                        method: 'PUT', // Correcto método HTTP: PUT
                        headers: {
                            'Content-Type': 'application/json', // Se sigue enviando JSON aunque sea un string
                            // 'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() // Descomentar si usas Anti-forgery tokens
                        },
                        // El cuerpo es SOLO el string del nuevo estatus, no un objeto JSON
                        body: JSON.stringify(newStatus) // JSON.stringify() convierte el string a un JSON string ("Pendiente")
                    })
                        .then(response => {
                            // Tu API devuelve 204 No Content para éxito.
                            // Si la respuesta no es OK (ej. 404, 500), entonces intenta leer JSON para el error.
                            if (!response.ok) {
                                // Intenta leer el mensaje de error del cuerpo de la respuesta, si lo hay
                                // Si tu API devuelve un problema JSON, intentará leerlo.
                                // Si es un 204, .json() fallará, pero no importa si response.ok es true.
                                return response.text().then(text => { // Usar .text() para ver el cuerpo si falla
                                    try {
                                        const errorJson = JSON.parse(text);
                                        throw new Error(errorJson.message || text || 'Error desconocido del servidor');
                                    } catch {
                                        throw new Error(text || 'Error desconocido del servidor');
                                    }
                                });
                            }
                            // Si la respuesta es 204 No Content, response.json() causará un error.
                            // Para 204, no necesitas parsear JSON. Solo verifica que response.ok sea true.
                            if (response.status === 204) {
                                return {}; // Devuelve un objeto vacío para que el siguiente .then() no falle
                            }
                            return response.json(); // Para otras respuestas exitosas con JSON
                        })
                        .then(data => {
                            // Dado que para 204 NoContent, devolvemos un objeto vacío, data.success no existirá.
                            // Solo verificamos si la respuesta original fue OK.
                            // Si la API regresa 204 No Content para éxito, la lógica es más simple:
                            Swal.fire(
                                '¡Actualizado!',
                                'El estatus de la cita ha sido actualizado.', // Mensaje genérico de éxito
                                'success'
                            ).then(() => {
                                var statusSpan = document.getElementById(`status-${citaId}`);
                                statusSpan.textContent = newStatus;

                                statusSpan.className = 'badge';
                                if (newStatus === "Pendiente") {
                                    statusSpan.classList.add('bg-warning', 'text-dark');
                                } else if (newStatus === "Confirmada") {
                                    statusSpan.classList.add('bg-success');
                                } else if (newStatus === "Cancelada") {
                                    statusSpan.classList.add('bg-danger');
                                } else { // Completada o cualquier otro
                                    statusSpan.classList.add('bg-info');
                                }

                                dropdown.value = newStatus;
                            });
                        })
                        .catch(error => {
                            console.error('Error:', error);
                            Swal.fire(
                                'Error en la solicitud',
                                'No se pudo conectar con el servidor o hubo un error inesperado: ' + error.message,
                                'error'
                            );
                        });
                }
            });
        });
    });
});