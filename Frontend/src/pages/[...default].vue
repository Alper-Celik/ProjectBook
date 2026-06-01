<script setup lang="ts">
import SvgIcon from '@jamescoyle/vue-icon'
import { mdiAlertCircle } from '@mdi/js'
</script>

<template>
  <header class="flex w-screen h-screen">
    <div class="alert-error alert h-min w-screen">
      <SvgIcon type="mdi" :path="mdiAlertCircle"></SvgIcon>
      <p class="text-nowrap">Not Found</p>
    </div>
  </header>
</template>
