<script setup lang="ts">
import { ref, computed } from 'vue'
import { useQuery, useMutation } from '@pinia/colada'
import { getApiAuthRegisterInfo, postApiAuthRegister } from '@/api'
import type { RegisterDto } from '@/api'

// Form state
const userHandle = ref('')
const email = ref('')
const password = ref('')
const confirmPassword = ref('')
const isAdmin = ref(false)

// UI state
const pageError = ref('')
const pageSuccess = ref('')

// Fetch register info (admin availability + regex)
const { data: registerInfo } = useQuery({
  key: ['register-info'],
  query: async () => {
    const { data, error } = await getApiAuthRegisterInfo()
    if (error) throw error
    return data
  },
})

const isAdminAvailable = computed(() => registerInfo.value?.canRegisterAsAdmin ?? false)
const userHandleRegex = computed(() => registerInfo.value?.userHandleAcceptedRegex ?? '')

// Registration mutation
const { mutateAsync: register, isLoading } = useMutation({
  mutation: async (body: RegisterDto) => {
    const { data, error } = await postApiAuthRegister({ body })
    if (error) throw error
    return data
  },
})

const showError = (message: string): void => {
  pageError.value = message
  pageSuccess.value = ''
}

const showSuccess = (message: string): void => {
  pageSuccess.value = message
  pageError.value = ''
}

const validateForm = (): boolean => {
  if (!userHandle.value.trim()) {
    showError('User handle is required')
    return false
  }
  if (userHandleRegex.value && !new RegExp(userHandleRegex.value).test(userHandle.value)) {
    showError('User handle format is invalid')
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

const handleRegister = async (): Promise<void> => {
  pageError.value = ''
  pageSuccess.value = ''

  if (!validateForm()) {
    return
  }

  try {
    await register({
      userHandle: userHandle.value,
      email: email.value || null,
      password: password.value,
      adminRegistration: isAdmin.value,
    })

    showSuccess('Registration successful!')

    // Reset form
    userHandle.value = ''
    email.value = ''
    password.value = ''
    confirmPassword.value = ''
    isAdmin.value = false
  } catch (err: any) {
    if (err?.status === 409) {
      showError('User handle or email already exists')
    } else {
      showError(err?.message ?? 'Registration failed. Please try again.')
    }
  }
}
</script>

<template>
  <div class="min-h-screen flex items-center justify-center bg-base-200 p-4">
    <div class="card w-full max-w-md bg-base-100 shadow-xl">
      <div class="card-body">
        <h2 class="card-title text-2xl font-bold text-center justify-center mb-6">
          Create Account
        </h2>

        <!-- Error Alert -->
        <div v-if="pageError" class="alert alert-error mb-4">
          <span>{{ pageError }}</span>
        </div>

        <!-- Success Alert -->
        <div v-if="pageSuccess" class="alert alert-success mb-4">
          <span>{{ pageSuccess }}</span>
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
              :disabled="isLoading"
            >
              <span v-if="isLoading" class="loading loading-spinner"></span>
              {{ isLoading ? 'Registering...' : 'Register' }}
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
