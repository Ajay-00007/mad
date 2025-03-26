package com.medibook.data

import com.medibook.ui.theme.ThemeUtils
import java.time.LocalDate
import java.time.LocalTime

data class Doctor(
    val id: Int,
    val name: String,
    val specialization: String,
    val experience: Int,
    val languages: List<String>,
    val certifications: List<String>,
    val imageUrl: String,
    val availability: List<String>
)

data class Clinic(
    val id: Int,
    val name: String,
    val address: String,
    val phone: String,
    val email: String,
    val website: String,
    val latitude: Double,
    val longitude: Double,
    val operatingHours: Map<String, String>,
    val specialties: List<String>,
    val facilities: List<String>
)

data class Appointment(
    val id: Int,
    val doctorId: Int,
    val patientName: String,
    val patientPhone: String,
    val patientGender: String,
    val isFirstVisit: Boolean,
    val reasonForVisit: String,
    val appointmentDate: LocalDate,
    val appointmentTime: LocalTime,
    val status: AppointmentStatus
)

enum class AppointmentStatus {
    PENDING,
    CONFIRMED,
    CANCELLED,
    COMPLETED
}

data class MedicalHistory(
    val chronicConditions: List<String> = emptyList(),
    val surgeries: List<String> = emptyList(),
    val medications: List<String> = emptyList(),
    val allergies: List<String> = emptyList(),
    val familyHistory: String = "",
    val bloodType: String = "",
    val height: String = "",
    val weight: String = "",
    val smokingStatus: String = "",
    val alcoholConsumption: String = "",
    val exerciseFrequency: String = "",
    val dietaryRestrictions: List<String> = emptyList(),
    val pregnancyStatus: Boolean? = null,
    val vaccinationHistory: List<String> = emptyList()
)