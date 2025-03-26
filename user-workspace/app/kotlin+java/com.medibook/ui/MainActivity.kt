package com.medibook.ui

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Surface
import com.medibook.data.Appointment
import com.medibook.ui.theme.MediBookTheme

class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContent {
            MediBookTheme {
                Surface {
                    AppointmentForm(
                        onSubmit = { appointment: Appointment ->
                            // Handle appointment submission
                        },
                        doctorId = 1 // Example doctor ID
                    )
                }
            }
        }
    }
}