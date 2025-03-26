package com.medibook.ui.components

import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import com.medibook.data.*
import java.time.LocalDate
import java.time.LocalTime

@Composable
fun AppointmentForm(
    onSubmit: (Appointment) -> Unit,
    doctorId: Int,
    modifier: Modifier = Modifier
) {
    var patientName by remember { mutableStateOf("") }
    var patientPhone by remember { mutableStateOf("") }
    var patientGender by remember { mutableStateOf("") }
    var isFirstVisit by remember { mutableStateOf(true) }
    var reasonForVisit by remember { mutableStateOf("") }
    var selectedDate by remember { mutableStateOf<LocalDate?>(null) }
    var selectedTime by remember { mutableStateOf<LocalTime?>(null) }

    Column(
        modifier = modifier.padding(16.dp),
        verticalArrangement = Arrangement.spacedBy(16.dp)
    ) {
        OutlinedTextField(
            value = patientName,
            onValueChange = { patientName = it },
            label = { Text("Full Name") },
            modifier = Modifier.fillMaxWidth()
        )

        OutlinedTextField(
            value = patientPhone,
            onValueChange = { patientPhone = it },
            label = { Text("Phone Number") },
            modifier = Modifier.fillMaxWidth()
        )

        // Gender Selection
        Column {
            Text("Gender")
            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.spacedBy(16.dp)
            ) {
                listOf("Male", "Female", "Other").forEach { gender ->
                    RadioButton(
                        selected = patientGender == gender,
                        onClick = { patientGender = gender }
                    )
                    Text(gender)
                }
            }
        }

        // First Visit Switch
        Row(
            modifier = Modifier.fillMaxWidth(),
            horizontalArrangement = Arrangement.SpaceBetween,
            verticalAlignment = Alignment.CenterVertically
        ) {
            Text("Is this your first visit?")
            Switch(
                checked = isFirstVisit,
                onCheckedChange = { isFirstVisit = it }
            )
        }

        OutlinedTextField(
            value = reasonForVisit,
            onValueChange = { reasonForVisit = it },
            label = { Text("Reason for Visit") },
            modifier = Modifier.fillMaxWidth(),
            minLines = 3
        )

        // Date and Time Selection
        DateTimeSelector(
            selectedDate = selectedDate,
            selectedTime = selectedTime,
            onDateSelected = { selectedDate = it },
            onTimeSelected = { selectedTime = it }
        )

        Button(
            onClick = {
                if (selectedDate != null && selectedTime != null) {
                    onSubmit(
                        Appointment(
                            id = 0, // Will be assigned by backend
                            doctorId = doctorId,
                            patientName = patientName,
                            patientPhone = patientPhone,
                            patientGender = patientGender,
                            isFirstVisit = isFirstVisit,
                            reasonForVisit = reasonForVisit,
                            appointmentDate = selectedDate,
                            appointmentTime = selectedTime,
                            status = AppointmentStatus.PENDING
                        )
                    )
                }
            },
            modifier = Modifier.fillMaxWidth(),
            enabled = patientName.isNotBlank() && patientPhone.isNotBlank() &&
                    patientGender.isNotBlank() && reasonForVisit.isNotBlank() &&
                    selectedDate != null && selectedTime != null
        ) {
            Text("Book Appointment")
        }
    }
}

@Composable
fun DateTimeSelector(
    selectedDate: LocalDate?,
    selectedTime: LocalTime?,
    onDateSelected: (LocalDate) -> Unit,
    onTimeSelected: (LocalTime) -> Unit
) {
    Column(
        verticalArrangement = Arrangement.spacedBy(8.dp)
    ) {
        Text("Select Date and Time")
        
        // Date selection
        LazyColumn(
            modifier = Modifier.height(200.dp),
            verticalArrangement = Arrangement.spacedBy(8.dp)
        ) {
            val dates = (0..14).map { LocalDate.now().plusDays(it.toLong()) }
            items(dates) { date ->
                DateCard(
                    date = date,
                    isSelected = date == selectedDate,
                    onClick = { onDateSelected(date) }
                )
            }
        }

        // Time selection
        if (selectedDate != null) {
            LazyColumn(
                modifier = Modifier.height(200.dp),
                verticalArrangement = Arrangement.spacedBy(8.dp)
            ) {
                val times = (9..17).map { LocalTime.of(it, 0) }
                items(times) { time ->
                    TimeCard(
                        time = time,
                        isSelected = time == selectedTime,
                        onClick = { onTimeSelected(time) }
                    )
                }
            }
        }
    }
}

@Composable
private fun DateCard(
    date: LocalDate,
    isSelected: Boolean,
    onClick: () -> Unit
) {
    Card(
        modifier = Modifier.fillMaxWidth(),
        colors = CardDefaults.cardColors(
            containerColor = if (isSelected) {
                MaterialTheme.colorScheme.primaryContainer
            } else {
                MaterialTheme.colorScheme.surface
            }
        ),
        onClick = onClick
    ) {
        Text(
            text = date.toString(),
            modifier = Modifier.padding(16.dp)
        )
    }
}

@Composable
private fun TimeCard(
    time: LocalTime,
    isSelected: Boolean,
    onClick: () -> Unit
) {
    Card(
        modifier = Modifier.fillMaxWidth(),
        colors = CardDefaults.cardColors(
            containerColor = if (isSelected) {
                MaterialTheme.colorScheme.primaryContainer
            } else {
                MaterialTheme.colorScheme.surface
            }
        ),
        onClick = onClick
    ) {
        Text(
            text = time.toString(),
            modifier = Modifier.padding(16.dp)
        )
    }
}

@Composable
fun LoadingSpinner(
    modifier: Modifier = Modifier
) {
    Box(
        modifier = modifier.fillMaxSize(),
        contentAlignment = Alignment.Center
    ) {
        CircularProgressIndicator()
    }
}

@Composable
fun ErrorMessage(
    message: String,
    onRetry: () -> Unit,
    modifier: Modifier = Modifier
) {
    Column(
        modifier = modifier.fillMaxSize(),
        horizontalAlignment = Alignment.CenterHorizontally,
        verticalArrangement = Arrangement.Center
    ) {
        Text(message)
        Button(onClick = onRetry) {
            Text("Retry")
        }
    }
}