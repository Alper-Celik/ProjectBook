<script setup lang="ts">
import { ref, onMounted } from 'vue'

// Form state
const userHandle = ref('')
const email = ref('')
const password = ref('')
const confirmPassword = ref('')
const isAdmin = ref(false)

// UI state
const isAdminAvailable = ref(false)
const loading = ref(false)
const error = ref('')
const success = ref('')

// Check if admin registration is available (empty function for now)
const checkAdminRegistrationAvailable = async (): Promise<boolean> => {
  // TODO: Implement backend check
  // Example: return await fetch('/api/auth/setup-status').then(r => r.json())
  return false
}

// Validate form fields (empty function for now)
const validateForm = (): boolean => {
  // TODO: Implement proper validation
  if (!userHandle.value.trim()) {
    showError('User handle is required')
    return false
  }
  if (!password.value) {
    showError('Password is required')
    return false
  }
  if (password.value !== confirmPassword.value) {
    showError('Passwords do not match')
    return false
  }
  return true
}

// Show error message (empty function for now)
const showError = (message: string): void => {
  error.value = message
  success.value = ''
  // TODO: Implement toast or persistent error display
}

// Show success message (empty function for now)
const showSuccess = (message: string): void => {
  success.value = message
  error.value = ''
  // TODO: Implement toast or persistent success display
}

// Handle form submission (empty function for now)
const handleRegister = async (): Promise<void> => {
  // TODO: Implement registration logic
  loading.value = true
  error.value = ''
  success.value = ''

  try {
    if (!validateForm()) {
      loading.value = false
      return
    }

    const payload = {
      userHandle: userHandle.value,
      email: email.value || null,
      password: password.value,
      isAdmin: isAdmin.value
    }

    // Example API call:
    // const response = await fetch('/api/auth/register', {
    //   method: 'POST',
    //   headers: { 'Content-Type': 'application/json' },
    //   body: JSON.stringify(payload)
    // })
    //
    // if (response.status === 409) {
    //   showError('User handle or email already exists')
    //   return
    // }
    //
    // if (!response.ok) {
    //   showError('Registration failed. Please try again.')
    //   return
    // }
    //
    // const userId = await response.json()
    // showSuccess('Registration successful!')

    console.log('Register payload:', payload)
    showSuccess('Registration successful! (Mock)')
  } catch (err) {
    showError('An unexpected error occurred.')
    console.error('Registration error:', err)
  } finally {
    loading.value = false
  }
}

// Check admin availability on mount
onMounted(async () => {
  isAdminAvailable.value = await checkAdminRegistrationAvailable()
})
</script>

<template>
  <div class="min-h-screen flex items-center justify-center bg-base-200 p-4">
    <div class="card w-full max-w-md bg-base-100 shadow-xl">
      <div class="card-body">
        <h2 class="card-title text-2xl font-bold text-center justify-center mb-6">
          Create Account
        </h2>

        <!-- Error Alert -->
        <div v-if="error" class="alert alert-error mb-4">
          <span>{{ error }}</span>
        </div>

        <!-- Success Alert -->
        <div v-if="success" class="alert alert-success mb-4">
          <span>{{ success }}</span>
        </div>

        <form @submit.prevent="handleRegister" class="space-y-4">
          <!-- User Handle -->
          <div class="form-control">
            <label class="label">
              <span class="label-text">User Handle</span>
              <span class="label-text-alt text-error">*</span>
            </label>
            <input
              v-model="userHandle"
              type="text"
              placeholder="Enter your handle"
              class="input input-bordered w-full"
              required
            />
          </div>

          <!-- Email -->
          <div class="form-control">
            <label class="label">
              <span class="label-text">Email</span>
              <span class="label-text-alt text-base-content/50">Optional</span>
            </label>
            <input
              v-model="email"
              type="email"
              placeholder="Enter your email"
              class="input input-bordered w-full"
            />
          </div>

          <!-- Password -->
          <div class="form-control">
            <label class="label">
              <span class="label-text">Password</span>
              <span class="label-text-alt text-error">*</span>
            </label>
            <input
              v-model="password"
              type="password"
              placeholder="Enter your password"
              class="input input-bordered w-full"
              required
            />
          </div>

          <!-- Confirm Password -->
          <div class="form-control">
            <label class="label">
              <span class="label-text">Confirm Password</span>
              <span class="label-text-alt text-error">*</span>
            </label>
            <input
              v-model="confirmPassword"
              type="password"
              placeholder="Confirm your password"
              class="input input-bordered w-full"
              required
            />
          </div>

          <!-- Admin Registration Checkbox -->
          <div v-if="isAdminAvailable" class="form-control">
            <label class="label cursor-pointer justify-start gap-3">
              <input
                v-model="isAdmin"
                type="checkbox"
                class="checkbox checkbox-primary"
              />
              <span class="label-text">Register as Administrator</span>
            </label>
          </div>

          <!-- Submit Button -->
          <div class="form-control mt-6">
            <button
              type="submit"
              class="btn btn-primary w-full"
              :disabled="loading"
            >
              <span v-if="loading" class="loading loading-spinner"></span>
              {{ loading ? 'Registering...' : 'Register' }}
            </button>
          </div>
        </form>

        <!-- Login Link -->
        <div class="divider">or</div>
        <div class="text-center">
          <p class="text-sm">
            Already have an account?
            <RouterLink to="/login" class="link link-primary">
              Sign in
            </RouterLink>
          </p>
        </div>
      </div>
    </div>
  </div>
</template>
