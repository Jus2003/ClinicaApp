using System.Text;
using System.Text.Json;
using ClinicaApp.Helpers;
using ClinicaApp.Models;
using static ClinicaApp.Models.CitaDetalle;

namespace ClinicaApp.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://93dc006b2926.ngrok-free.app/citas-medicas-api/public";

        public ApiService()
        {
            _httpClient = new HttpClient();
            // Headers importantes para ngrok
            _httpClient.DefaultRequestHeaders.Add("ngrok-skip-browser-warning", "true");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "ClinicaApp/1.0");
        }

        public async Task<ApiResponse<LoginResponse>> LoginAsync(string usuario, string password)
        {
            try
            {
                var loginRequest = new LoginRequest
                {
                    Usuario = usuario,
                    Password = password
                };

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var json = JsonSerializer.Serialize(loginRequest, options);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseUrl}/auth/login", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"Response Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"Response Content: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    var deserializeOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    return JsonSerializer.Deserialize<ApiResponse<LoginResponse>>(responseContent, deserializeOptions);
                }
                else
                {
                    // Tratar de deserializar el error
                    try
                    {
                        var errorOptions = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        };
                        var errorResponse = JsonSerializer.Deserialize<ApiResponse<LoginResponse>>(responseContent, errorOptions);
                        return errorResponse;
                    }
                    catch
                    {
                        return new ApiResponse<LoginResponse>
                        {
                            Success = false,
                            Message = $"Error del servidor: {response.StatusCode}",
                            Status = (int)response.StatusCode
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<LoginResponse>
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}",
                    Status = 500
                };
            }
        }

        // Agregar este método a tu clase ApiService existente
        public async Task<ApiResponse<object>> LogoutAsync()
        {
            try
            {
                var response = await _httpClient.PostAsync($"{_baseUrl}/auth/logout", null);
                var responseContent = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"Logout Response Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"Logout Response Content: {responseContent}");

                var deserializeOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<ApiResponse<object>>(responseContent, deserializeOptions);
                }
                else
                {
                    return new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Error en logout del servidor",
                        Status = (int)response.StatusCode
                    };
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en logout: {ex.Message}");
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Error de conexión en logout: {ex.Message}",
                    Status = 500
                };
            }
        }

        // MÉTODO ACTUALIZADO PARA FORGOT PASSWORD
        public async Task<ApiResponse<ForgotPasswordResponse>> ForgotPasswordAsync(string email)
        {
            try
            {
                var request = new ForgotPasswordRequest { Email = email };

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var json = JsonSerializer.Serialize(request, options);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseUrl}/auth/olvido-password", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"ForgotPassword Response Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"ForgotPassword Response Content: {responseContent}");

                var deserializeOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<ApiResponse<ForgotPasswordResponse>>(responseContent, deserializeOptions);
                }
                else
                {
                    // Manejar errores
                    try
                    {
                        var errorResponse = JsonSerializer.Deserialize<ApiResponse<ForgotPasswordResponse>>(responseContent, deserializeOptions);
                        return errorResponse;
                    }
                    catch
                    {
                        return new ApiResponse<ForgotPasswordResponse>
                        {
                            Success = false,
                            Message = $"Error del servidor: {response.StatusCode}",
                            Status = (int)response.StatusCode
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<ForgotPasswordResponse>
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}",
                    Status = 500
                };
            }
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/bypass-test");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }


        public async Task<ApiResponse<List<Especialidad>>> GetEspecialidadesAsync()
        {
            try
            {
                await Task.Delay(500); // Simular delay de red

                return new ApiResponse<List<Especialidad>>
                {
                    Success = true,
                    Message = "Especialidades cargadas exitosamente",
                    Status = 200,
                    Data = new List<Especialidad>
            {
                new Especialidad { IdEspecialidad = 1, NombreEspecialidad = "Medicina General", Descripcion = "Atención médica integral y preventiva", Activo = true },
                new Especialidad { IdEspecialidad = 2, NombreEspecialidad = "Cardiología", Descripcion = "Especialidad en enfermedades del corazón", Activo = true },
                new Especialidad { IdEspecialidad = 3, NombreEspecialidad = "Dermatología", Descripcion = "Especialidad en enfermedades de la piel", Activo = true },
                new Especialidad { IdEspecialidad = 4, NombreEspecialidad = "Pediatría", Descripcion = "Atención médica especializada en niños", Activo = true },
                new Especialidad { IdEspecialidad = 5, NombreEspecialidad = "Ginecología", Descripcion = "Especialidad en salud femenina", Activo = true },
                new Especialidad { IdEspecialidad = 6, NombreEspecialidad = "Traumatología", Descripcion = "Especialidad en lesiones y fracturas", Activo = true },
                new Especialidad { IdEspecialidad = 7, NombreEspecialidad = "Psicología", Descripcion = "Atención en salud mental", Activo = true },
                new Especialidad { IdEspecialidad = 8, NombreEspecialidad = "Nutrición", Descripcion = "Especialidad en alimentación y dietas", Activo = true },
                new Especialidad { IdEspecialidad = 9, NombreEspecialidad = "Oftalmología", Descripcion = "Especialidad en enfermedades de los ojos", Activo = true },
                new Especialidad { IdEspecialidad = 10, NombreEspecialidad = "Odontología", Descripcion = "Especialidad en salud dental", Activo = true },
                new Especialidad { IdEspecialidad = 11, NombreEspecialidad = "Podología", Descripcion = "Consultas detalladas sobre la salud de los pies", Activo = true },
                new Especialidad { IdEspecialidad = 12, NombreEspecialidad = "Traumatología Avanzada", Descripcion = "Tratamiento de traumas", Activo = true }
            }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<Especialidad>>
                {
                    Success = false,
                    Message = $"Error: {ex.Message}",
                    Status = 500
                };
            }
        }

        public async Task<ApiResponse<List<Sucursal>>> GetSucursalesAsync()
        {
            try
            {
                await Task.Delay(500); // Simular delay de red

                return new ApiResponse<List<Sucursal>>
                {
                    Success = true,
                    Message = "Sucursales cargadas exitosamente",
                    Status = 200,
                    Data = new List<Sucursal>
            {
                new Sucursal { IdSucursal = 1, NombreSucursal = "Clínica Central Quito", Activo = true },
                new Sucursal { IdSucursal = 2, NombreSucursal = "Clínica Norte", Activo = true },
                new Sucursal { IdSucursal = 3, NombreSucursal = "Clínica Sur", Activo = true },
                new Sucursal { IdSucursal = 4, NombreSucursal = "Clínica Valle de los Chillos", Activo = true }
            }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<Sucursal>>
                {
                    Success = false,
                    Message = $"Error: {ex.Message}",
                    Status = 500
                };
            }
        }

        public async Task<ApiResponse<DoctorResponse>> CreateDoctorAsync(Doctor doctor)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var json = JsonSerializer.Serialize(doctor, options);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                System.Diagnostics.Debug.WriteLine($"CreateDoctor Request JSON: {json}");

                var response = await _httpClient.PostAsync($"{_baseUrl}/medicos/crear", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"CreateDoctor Response Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"CreateDoctor Response Content: {responseContent}");

                var deserializeOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new StringToIntConverter() }  // Agregar el converter
                };

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<ApiResponse<DoctorResponse>>(responseContent, deserializeOptions);
                }
                else
                {
                    try
                    {
                        var errorResponse = JsonSerializer.Deserialize<ApiResponse<DoctorResponse>>(responseContent, deserializeOptions);
                        return errorResponse;
                    }
                    catch
                    {
                        return new ApiResponse<DoctorResponse>
                        {
                            Success = false,
                            Message = $"Error del servidor: {response.StatusCode}",
                            Status = (int)response.StatusCode
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en CreateDoctorAsync: {ex}");
                return new ApiResponse<DoctorResponse>
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}",
                    Status = 500
                };
            }
        }

        public async Task<ApiResponse<PatientResponse>> CreatePatientAsync(Patient patient)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var json = JsonSerializer.Serialize(patient, options);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                System.Diagnostics.Debug.WriteLine($"CreatePatient Request JSON: {json}");

                var response = await _httpClient.PostAsync($"{_baseUrl}/pacientes/crear", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"CreatePatient Response Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"CreatePatient Response Content: {responseContent}");

                var deserializeOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new StringToIntConverter() }  // Agregar el converter aquí también
                };

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<ApiResponse<PatientResponse>>(responseContent, deserializeOptions);
                }
                else
                {
                    try
                    {
                        var errorResponse = JsonSerializer.Deserialize<ApiResponse<PatientResponse>>(responseContent, deserializeOptions);
                        return errorResponse;
                    }
                    catch
                    {
                        return new ApiResponse<PatientResponse>
                        {
                            Success = false,
                            Message = $"Error del servidor: {response.StatusCode}",
                            Status = (int)response.StatusCode
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en CreatePatientAsync: {ex}");
                return new ApiResponse<PatientResponse>
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}",
                    Status = 500
                };
            }
        }

    public async Task<ApiResponse<List<MedicoBasico>>> GetMedicosAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/medicos/listar");
                var responseContent = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"GetMedicos Response: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        Converters = { new StringToIntConverter() }
                    };

                    return JsonSerializer.Deserialize<ApiResponse<List<MedicoBasico>>>(responseContent, options);
                }
                else
                {
                    return new ApiResponse<List<MedicoBasico>>
                    {
                        Success = false,
                        Message = $"Error del servidor: {response.StatusCode}",
                        Status = (int)response.StatusCode
                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<MedicoBasico>>
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}",
                    Status = 500
                };
            }
        }

        public async Task<ApiResponse<HorariosResponse>> GetHorariosMedicoAsync(int idMedico)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/medicos/{idMedico}/horarios");
                var responseContent = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"GetHorarios Response: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        Converters = { new StringToIntConverter() }
                    };

                    return JsonSerializer.Deserialize<ApiResponse<HorariosResponse>>(responseContent, options);
                }
                else
                {
                    return new ApiResponse<HorariosResponse>
                    {
                        Success = false,
                        Message = $"Error del servidor: {response.StatusCode}",
                        Status = (int)response.StatusCode
                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<HorariosResponse>
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}",
                    Status = 500
                };
            }
        }

        public async Task<ApiResponse<object>> AsignarHorariosAsync(int idMedico, List<HorarioRequest> horarios)
        {
            try
            {
                var request = new AsignarHorariosRequest { Horarios = horarios };

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var json = JsonSerializer.Serialize(request, options);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                System.Diagnostics.Debug.WriteLine($"AsignarHorarios Request: {json}");

                var response = await _httpClient.PostAsync($"{_baseUrl}/medicos/{idMedico}/horarios", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"AsignarHorarios Response: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    var deserializeOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    return JsonSerializer.Deserialize<ApiResponse<object>>(responseContent, deserializeOptions);
                }
                else
                {
                    try
                    {
                        var errorOptions = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        };
                        return JsonSerializer.Deserialize<ApiResponse<object>>(responseContent, errorOptions);
                    }
                    catch
                    {
                        return new ApiResponse<object>
                        {
                            Success = false,
                            Message = $"Error del servidor: {response.StatusCode}",
                            Status = (int)response.StatusCode
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}",
                    Status = 500
                };
            }
        }

        // Agregar estos métodos al ApiService existente

        public async Task<ApiResponse<List<Especialidad>>> GetEspecialidadesPorTipoAsync(string tipoCita)
        {
            try
            {
                // Por ahora usar datos hardcodeados con filtro por tipo
                await Task.Delay(500);

                var todasEspecialidades = new List<Especialidad>
        {
            new Especialidad { IdEspecialidad = 1, NombreEspecialidad = "Medicina General", Activo = true },
            new Especialidad { IdEspecialidad = 2, NombreEspecialidad = "Cardiología", Activo = true },
            new Especialidad { IdEspecialidad = 3, NombreEspecialidad = "Dermatología", Activo = true },
            new Especialidad { IdEspecialidad = 4, NombreEspecialidad = "Pediatría", Activo = true },
            new Especialidad { IdEspecialidad = 5, NombreEspecialidad = "Ginecología", Activo = true },
            new Especialidad { IdEspecialidad = 6, NombreEspecialidad = "Traumatología", Activo = true },
            new Especialidad { IdEspecialidad = 7, NombreEspecialidad = "Psicología", Activo = true },
            new Especialidad { IdEspecialidad = 8, NombreEspecialidad = "Nutrición", Activo = true },
            new Especialidad { IdEspecialidad = 9, NombreEspecialidad = "Oftalmología", Activo = true },
            new Especialidad { IdEspecialidad = 10, NombreEspecialidad = "Odontología", Activo = true },
            new Especialidad { IdEspecialidad = 11, NombreEspecialidad = "Podología", Activo = true },
            new Especialidad { IdEspecialidad = 12, NombreEspecialidad = "Traumatología Avanzada", Activo = true }
        };

                // Filtrar especialidades según tipo de cita
                var especialidadesFiltradas = tipoCita == "virtual"
                    ? todasEspecialidades.Where(e => new[] { 1, 2, 5, 7, 8 }.Contains(e.IdEspecialidad)).ToList()
                    : todasEspecialidades;

                return new ApiResponse<List<Especialidad>>
                {
                    Success = true,
                    Message = $"Especialidades para citas {tipoCita}",
                    Data = especialidadesFiltradas
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<Especialidad>>
                {
                    Success = false,
                    Message = $"Error: {ex.Message}",
                    Status = 500
                };
            }
        }

        public async Task<ApiResponse<MedicosEspecialidadResponse>> GetMedicosPorEspecialidadAsync(int idEspecialidad, string tipoCita)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/citas/medicos-por-especialidad/{idEspecialidad}/{tipoCita}");
                var responseContent = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"GetMedicos Response: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        Converters = { new StringToIntConverter() }
                    };

                    return JsonSerializer.Deserialize<ApiResponse<MedicosEspecialidadResponse>>(responseContent, options);
                }
                else
                {
                    return new ApiResponse<MedicosEspecialidadResponse>
                    {
                        Success = false,
                        Message = $"Error del servidor: {response.StatusCode}",
                        Status = (int)response.StatusCode
                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<MedicosEspecialidadResponse>
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}",
                    Status = 500
                };
            }
        }

        public async Task<ApiResponse<HorariosDisponiblesResponse>> GetHorariosDisponiblesAsync(int idMedico, string fecha, int duracionMinutos = 45)
        {
            try
            {
                var request = new HorariosDisponiblesRequest
                {
                    IdMedico = idMedico,
                    Fecha = fecha,
                    DuracionMinutos = duracionMinutos
                };

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var json = JsonSerializer.Serialize(request, options);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseUrl}/citas/horarios-disponibles", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"GetHorarios Request: {json}");
                System.Diagnostics.Debug.WriteLine($"GetHorarios Response: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    var deserializeOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        Converters = { new StringToIntConverter() }
                    };

                    return JsonSerializer.Deserialize<ApiResponse<HorariosDisponiblesResponse>>(responseContent, deserializeOptions);
                }
                else
                {
                    return new ApiResponse<HorariosDisponiblesResponse>
                    {
                        Success = false,
                        Message = $"Error del servidor: {response.StatusCode}",
                        Status = (int)response.StatusCode
                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<HorariosDisponiblesResponse>
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}",
                    Status = 500
                };
            }
        }

        public async Task<ApiResponse<ValidarDisponibilidadResponse>> ValidarDisponibilidadAsync(int idMedico, string fechaCita, string horaCita)
        {
            try
            {
                var request = new ValidarDisponibilidadRequest
                {
                    IdMedico = idMedico,
                    FechaCita = fechaCita,
                    HoraCita = horaCita
                };

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var json = JsonSerializer.Serialize(request, options);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseUrl}/citas/validar-disponibilidad", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                var deserializeOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new StringToIntConverter() }
                };

                return JsonSerializer.Deserialize<ApiResponse<ValidarDisponibilidadResponse>>(responseContent, deserializeOptions);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ValidarDisponibilidadResponse>
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}",
                    Status = 500
                };
            }
        }

        public async Task<ApiResponse<CitaResponse>> CrearCitaAsync(CitaRequest cita)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var json = JsonSerializer.Serialize(cita, options);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                System.Diagnostics.Debug.WriteLine($"CrearCita Request: {json}");

                var response = await _httpClient.PostAsync($"{_baseUrl}/citas/crear", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"CrearCita Response: {responseContent}");

                var deserializeOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new StringToIntConverter() }
                };

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<ApiResponse<CitaResponse>>(responseContent, deserializeOptions);
                }
                else
                {
                    try
                    {
                        return JsonSerializer.Deserialize<ApiResponse<CitaResponse>>(responseContent, deserializeOptions);
                    }
                    catch
                    {
                        return new ApiResponse<CitaResponse>
                        {
                            Success = false,
                            Message = $"Error del servidor: {response.StatusCode}",
                            Status = (int)response.StatusCode
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<CitaResponse>
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}",
                    Status = 500
                };
            }
        }

        public async Task<ApiResponse<PatientResponse>> BuscarPacientePorCedulaAsync(string cedula)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/pacientes/buscar-cedula/{cedula}");
                var responseContent = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"BuscarPaciente Response: {responseContent}");

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new StringToIntConverter() }
                };

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<ApiResponse<PatientResponse>>(responseContent, options);
                }
                else
                {
                    return new ApiResponse<PatientResponse>
                    {
                        Success = false,
                        Message = "Paciente no encontrado",
                        Status = (int)response.StatusCode
                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<PatientResponse>
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}",
                    Status = 500
                };
            }
        }


        // Agregar estos métodos a la clase ApiService existente

        public async Task<ApiResponse<dynamic>> GetCitasPorMedicoAsync(int idMedico)
        {
            try
            {
                var requestData = new { id_medico = idMedico };
                var json = JsonSerializer.Serialize(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseUrl}/citas/buscar-por-medico", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"GetCitasPorMedico Response: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonDoc = JsonDocument.Parse(responseContent);
                    return new ApiResponse<dynamic>
                    {
                        Success = jsonDoc.RootElement.GetProperty("success").GetBoolean(),
                        Message = jsonDoc.RootElement.GetProperty("message").GetString(),
                        Status = jsonDoc.RootElement.GetProperty("status").GetInt32(),
                        Data = jsonDoc.RootElement.GetProperty("data")
                    };
                }
                else
                {
                    return new ApiResponse<dynamic>
                    {
                        Success = false,
                        Message = $"Error del servidor: {response.StatusCode}",
                        Status = (int)response.StatusCode
                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<dynamic>
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}",
                    Status = 500
                };
            }
        }

        public async Task<ApiResponse<dynamic>> GetCitasPorPacienteAsync(int idPaciente)
        {
            try
            {
                var requestData = new { id_paciente = idPaciente };
                var json = JsonSerializer.Serialize(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseUrl}/citas/buscar-por-paciente", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"GetCitasPorPaciente Response: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonDoc = JsonDocument.Parse(responseContent);
                    return new ApiResponse<dynamic>
                    {
                        Success = jsonDoc.RootElement.GetProperty("success").GetBoolean(),
                        Message = jsonDoc.RootElement.GetProperty("message").GetString(),
                        Status = jsonDoc.RootElement.GetProperty("status").GetInt32(),
                        Data = jsonDoc.RootElement.GetProperty("data")
                    };
                }
                else
                {
                    return new ApiResponse<dynamic>
                    {
                        Success = false,
                        Message = $"Error del servidor: {response.StatusCode}",
                        Status = (int)response.StatusCode
                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<dynamic>
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}",
                    Status = 500
                };
            }
        }

        public async Task<ApiResponse<dynamic>> CambiarEstadoCitaAsync(int idCita, string nuevoEstado, string motivoCambio = null)
        {
            try
            {
                object requestData;

                if (!string.IsNullOrEmpty(motivoCambio))
                {
                    requestData = new { nuevo_estado = nuevoEstado, motivo_cambio = motivoCambio };
                }
                else
                {
                    requestData = new { nuevo_estado = nuevoEstado };
                }

                var json = JsonSerializer.Serialize(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseUrl}/citas/cambiar-estado/{idCita}", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"CambiarEstadoCita Response: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonDoc = JsonDocument.Parse(responseContent);
                    return new ApiResponse<dynamic>
                    {
                        Success = jsonDoc.RootElement.GetProperty("success").GetBoolean(),
                        Message = jsonDoc.RootElement.GetProperty("message").GetString(),
                        Status = jsonDoc.RootElement.GetProperty("status").GetInt32(),
                        Data = jsonDoc.RootElement.TryGetProperty("data", out var data) ? data : (JsonElement?)null
                    };
                }
                else
                {
                    return new ApiResponse<dynamic>
                    {
                        Success = false,
                        Message = $"Error del servidor: {response.StatusCode}",
                        Status = (int)response.StatusCode
                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<dynamic>
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}",
                    Status = 500
                };
            }
        }



        // Agregar estos métodos al ApiService.cs existente

        // Métodos para Triaje Digital
        public async Task<ApiResponse<List<PreguntaTriaje>>> GetPreguntasTriajeAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/triaje/preguntas");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    return JsonSerializer.Deserialize<ApiResponse<List<PreguntaTriaje>>>(responseContent, options);
                }
                else
                {
                    return new ApiResponse<List<PreguntaTriaje>>
                    {
                        Success = false,
                        Message = $"Error del servidor: {response.StatusCode}",
                        Status = (int)response.StatusCode
                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<PreguntaTriaje>>
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}",
                    Status = 500
                };
            }
        }

        public async Task<ApiResponse<EstadoTriaje>> VerificarEstadoTriajeAsync(int idCita)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/triaje/verificar/{idCita}");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    return JsonSerializer.Deserialize<ApiResponse<EstadoTriaje>>(responseContent, options);
                }
                else
                {
                    return new ApiResponse<EstadoTriaje>
                    {
                        Success = false,
                        Message = $"Error del servidor: {response.StatusCode}",
                        Status = (int)response.StatusCode
                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<EstadoTriaje>
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}",
                    Status = 500
                };
            }
        }

        public async Task<ApiResponse<object>> ResponderTriajeAsync(TriajeRequest request)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var json = JsonSerializer.Serialize(request, options);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseUrl}/triaje/responder", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var deserializeOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    return JsonSerializer.Deserialize<ApiResponse<object>>(responseContent, deserializeOptions);
                }
                else
                {
                    try
                    {
                        var errorOptions = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        };
                        return JsonSerializer.Deserialize<ApiResponse<object>>(responseContent, errorOptions);
                    }
                    catch
                    {
                        return new ApiResponse<object>
                        {
                            Success = false,
                            Message = $"Error del servidor: {response.StatusCode}",
                            Status = (int)response.StatusCode
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}",
                    Status = 500
                };
            }
        }

        public async Task<ApiResponse<object>> ObtenerTriajeCompletadoAsync(int idCita)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== API CALL TRIAJE COMPLETADO ===");
                System.Diagnostics.Debug.WriteLine($"URL: {_baseUrl}/triaje/cita/{idCita}");

                var response = await _httpClient.GetAsync($"{_baseUrl}/triaje/cita/{idCita}");
                var responseContent = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"Response Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"Response Content: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    return JsonSerializer.Deserialize<ApiResponse<object>>(responseContent, options);
                }
                else
                {
                    return new ApiResponse<object>
                    {
                        Success = false,
                        Message = $"Error del servidor: {response.StatusCode}",
                        Status = (int)response.StatusCode
                    };
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception en ObtenerTriajeCompletadoAsync: {ex.Message}");
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}",
                    Status = 500
                };
            }
        }

        // En ApiService.cs, en el método GetCitasPacienteAsync, agregar debug:

        public async Task<ApiResponse<CitasPacienteResponse>> GetCitasPacienteAsync(int idPaciente)
        {
            try
            {
                var request = new CitasPacienteRequest { IdPaciente = idPaciente };

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var json = JsonSerializer.Serialize(request, options);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // ✅ AGREGAR DEBUG
                System.Diagnostics.Debug.WriteLine($"=== API CALL ===");
                System.Diagnostics.Debug.WriteLine($"URL: {_baseUrl}/citas/buscar-por-paciente");
                System.Diagnostics.Debug.WriteLine($"Request JSON: {json}");
                System.Diagnostics.Debug.WriteLine($"ID Paciente enviado: {idPaciente}");

                var response = await _httpClient.PostAsync($"{_baseUrl}/citas/buscar-por-paciente", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                // ✅ AGREGAR MÁS DEBUG
                System.Diagnostics.Debug.WriteLine($"Response Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"Response Content: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    var deserializeOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<CitasPacienteResponse>>(responseContent, deserializeOptions);

                    // ✅ DEBUG DE LA RESPUESTA DESERIALIZADA
                    System.Diagnostics.Debug.WriteLine($"API Success: {apiResponse?.Success}");
                    System.Diagnostics.Debug.WriteLine($"API Message: {apiResponse?.Message}");
                    System.Diagnostics.Debug.WriteLine($"Citas Count: {apiResponse?.Data?.Citas?.Count}");

                    return apiResponse;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"ERROR: Status Code {response.StatusCode}");
                    return new ApiResponse<CitasPacienteResponse>
                    {
                        Success = false,
                        Message = $"Error del servidor: {response.StatusCode}",
                        Status = (int)response.StatusCode
                    };
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"EXCEPTION: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"STACK TRACE: {ex.StackTrace}");
                return new ApiResponse<CitasPacienteResponse>
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}",
                    Status = 500
                };
            }
        }

        public async Task<ApiResponse<CitaDetalladaResponse>> GetCitaDetalladaAsync(int idCita)
        {
            try
            {
                var request = new { id_cita = idCita };

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var json = JsonSerializer.Serialize(request, options);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseUrl}/citas/buscar-por-id", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var deserializeOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    return JsonSerializer.Deserialize<ApiResponse<CitaDetalladaResponse>>(responseContent, deserializeOptions);
                }
                else
                {
                    return new ApiResponse<CitaDetalladaResponse>
                    {
                        Success = false,
                        Message = $"Error del servidor: {response.StatusCode}",
                        Status = (int)response.StatusCode
                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<CitaDetalladaResponse>
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}",
                    Status = 500
                };
            }
        }

        public async Task<ApiResponse<TriajeCompletadoResponse>> GetTriajePorCitaAsync(int idCita)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== API CALL TRIAJE COMPLETADO ===");
                System.Diagnostics.Debug.WriteLine($"URL: {_baseUrl}/triaje/cita/{idCita}");

                var response = await _httpClient.GetAsync($"{_baseUrl}/triaje/cita/{idCita}");
                var responseContent = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"Response Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"Response Content: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    return JsonSerializer.Deserialize<ApiResponse<TriajeCompletadoResponse>>(responseContent, options);
                }
                else
                {
                    return new ApiResponse<TriajeCompletadoResponse>
                    {
                        Success = false,
                        Message = $"Error del servidor: {response.StatusCode}",
                        Status = (int)response.StatusCode
                    };
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception en GetTriajePorCitaAsync: {ex.Message}");
                return new ApiResponse<TriajeCompletadoResponse>
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}",
                    Status = 500
                };
            }
        }


        // ✅ AGREGAR ESTOS MÉTODOS AL ApiService EXISTENTE

        /// <summary>
        /// Obtener citas del médico logueado
        /// </summary>
        public async Task<ApiResponse<DoctorAppointmentsResponse>> GetDoctorAppointmentsAsync(int idMedico)
        {
            try
            {
                var request = new DoctorAppointmentsRequest { IdMedico = idMedico };
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseUrl}/citas/buscar-por-medico", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<ApiResponse<DoctorAppointmentsResponse>>(responseContent, options);
                }

                return new ApiResponse<DoctorAppointmentsResponse>
                {
                    Success = false,
                    Message = $"Error: {response.StatusCode}",
                    Status = (int)response.StatusCode
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<DoctorAppointmentsResponse>
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}",
                    Status = 500
                };
            }
        }

        /// <summary>
        /// Obtener detalles de una cita específica
        /// </summary>
        public async Task<ApiResponse<AppointmentDetailResponse>> GetAppointmentDetailAsync(int idCita)
        {
            try
            {
                var request = new AppointmentDetailRequest { IdCita = idCita };
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseUrl}/citas/buscar-por-id", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<ApiResponse<AppointmentDetailResponse>>(responseContent, options);
                }

                return new ApiResponse<AppointmentDetailResponse>
                {
                    Success = false,
                    Message = $"Error: {response.StatusCode}",
                    Status = (int)response.StatusCode
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<AppointmentDetailResponse>
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}",
                    Status = 500
                };
            }
        }

        /// <summary>
        /// Cambiar estado de una cita
        /// </summary>
        public async Task<ApiResponse<object>> ChangeAppointmentStatusAsync(int idCita, ChangeAppointmentStatusRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseUrl}/citas/cambiar-estado/{idCita}", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<ApiResponse<object>>(responseContent, options);
                }

                return new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Error: {response.StatusCode}",
                    Status = (int)response.StatusCode
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}",
                    Status = 500
                };
            }
        }

        /// <summary>
        /// Crear receta médica para una cita
        /// </summary>
        public async Task<ApiResponse<object>> CreatePrescriptionAsync(CreatePrescriptionRequest request)
        {
            try
            {
                // ✅ DEBUG - Mostrar la URL y datos que se envían
                var requestUrl = $"{_baseUrl}/recetas/crear";
                var json = JsonSerializer.Serialize(request);

                // Log para debug
                System.Diagnostics.Debug.WriteLine($"🔵 RECETA REQUEST URL: {requestUrl}");
                System.Diagnostics.Debug.WriteLine($"🔵 RECETA REQUEST JSON: {json}");

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(requestUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                // ✅ DEBUG - Mostrar la respuesta del servidor
                System.Diagnostics.Debug.WriteLine($"🔴 RECETA RESPONSE STATUS: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"🔴 RECETA RESPONSE CONTENT: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<ApiResponse<object>>(responseContent, options);
                }

                return new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Error HTTP {response.StatusCode}: {responseContent}",
                    Status = (int)response.StatusCode
                };
            }
            catch (Exception ex)
            {
                // ✅ DEBUG - Mostrar errores de conexión
                System.Diagnostics.Debug.WriteLine($"❌ RECETA EXCEPTION: {ex.Message}");

                return new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}",
                    Status = 500
                };
            }
        }




    }

}