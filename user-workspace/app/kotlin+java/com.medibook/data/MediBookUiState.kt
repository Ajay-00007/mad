package com.medibook.data

sealed class MediBookUiState {
    object Loading : MediBookUiState()
    data class Success(
        val doctors: List<Doctor> = emptyList(),
        val clinics: List<Clinic> = emptyList(),
        val appointments: List<Appointment> = emptyList(),
        val selectedDoctor: Doctor? = null,
        val selectedClinic: Clinic? = null,
        val selectedAppointment: Appointment? = null,
        val searchQuery: String = "",
        val selectedSpecialization: String? = null,
        val selectedLanguages: List<String> = emptyList(),
        val showAvailableOnly: Boolean = false,
        val sortOption: SortOption = SortOption.RELEVANCE,
        val userLocation: Location? = null
    ) : MediBookUiState()
    data class Error(val message: String) : MediBookUiState()
}

data class Location(
    val latitude: Double,
    val longitude: Double
)

enum class SortOption(val displayName: String) {
    RELEVANCE("Relevance"),
    EXPERIENCE("Experience"),
    RATING("Rating"),
    DISTANCE("Distance")
}

sealed class ViewEvent {
    data class ShowSnackbar(val message: String) : ViewEvent()
    data class ShowToast(val message: String) : ViewEvent()
    data class Navigate(val route: String) : ViewEvent()
    object NavigateBack : ViewEvent()
    object Refresh : ViewEvent()
}