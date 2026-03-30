import { createBrowserRouter } from 'react-router-dom'
import LoginPage from '../features/auth/LoginPage'
import DashboardPage from '../features/dashboard/DashboardPage'
import AgendaPage from '../features/agenda/AgendaPage'
import BookingLandingPage from '../features/public/BookingLandingPage'
import { AppShell } from '../components/layout/AppShell'
import { AuthGuard } from '../hooks/useAuthGuard'

export const router = createBrowserRouter([
  { path: '/', element: <BookingLandingPage /> },
  { path: '/login', element: <LoginPage /> },
  {
    path: '/app',
    element: (
      <AuthGuard>
        <AppShell />
      </AuthGuard>
    ),
    children: [
      { index: true, element: <DashboardPage /> },
      { path: 'agenda', element: <AgendaPage /> },
    ],
  },
])