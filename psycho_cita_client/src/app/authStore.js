import { create } from 'zustand'

export const useAuthStore = create((set) => ({
  token: localStorage.getItem('pc_token') || null,
  user: JSON.parse(localStorage.getItem('pc_user') || 'null'),

  login: ({ token, user }) => {
    localStorage.setItem('pc_token', token)
    localStorage.setItem('pc_user', JSON.stringify(user))
    set({ token, user })
  },

  logout: () => {
    localStorage.removeItem('pc_token')
    localStorage.removeItem('pc_user')
    set({ token: null, user: null })
  },
}))