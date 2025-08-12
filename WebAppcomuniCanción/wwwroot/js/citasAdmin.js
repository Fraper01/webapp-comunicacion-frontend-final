document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.status-dropdown').forEach(function (dropdown) {
        dropdown.addEventListener('change', function () {
            var citaId = this.dataset.citaId;
            var newStatus = this.value;

            if (newStatus === "") {
                return; 
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
                    fetch(`http://localhost:5000/api/Citas/${citaId}/estatus`, { 
                        method: 'PUT', 
                        headers: {
                            'Content-Type': 'application/json', 
                        },
                        body: JSON.stringify(newStatus) 
                    })
                        .then(response => {
                            if (!response.ok) {
                                return response.text().then(text => { 
                                    try {
                                        const errorJson = JSON.parse(text);
                                        throw new Error(errorJson.message || text || 'Error desconocido del servidor');
                                    } catch {
                                        throw new Error(text || 'Error desconocido del servidor');
                                    }
                                });
                            }
                            if (response.status === 204) {
                                return {}; 
                            }
                            return response.json(); 
                        })
                        .then(data => {
                            Swal.fire(
                                '¡Actualizado!',
                                'El estatus de la cita ha sido actualizado.', 
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
                                } else { 
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