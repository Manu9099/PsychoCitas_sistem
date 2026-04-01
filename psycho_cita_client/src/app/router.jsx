import { createBrowserRouter } from 'react-router-dom'

import LoginPage from '../features/auth/LoginPage'
import DashboardPage from '../features/dashboard/DashboardPage'
import AgendaPage from '../features/agenda/AgendaPage'
import BookingLandingPage from '../features/public/BookingLandingPage'
import BookingFlowPage from '../features/public/BookingFlowPage'
import CheckoutResultPage from '../features/public/CheckoutResultPage'
import PacientesPage from '../features/pacientes/PacientesPage'
import PacienteDetallePage from '../features/pacientes/PacienteDetallePage'
import PagoPage from '../features/pagos/PagoPage'
import NotificacionesPage from '../features/notificaciones/NotificacionesPage'
import { AppShell } from '../components/layout/AppShell'
import { AuthGuard } from '../hooks/useAuthGuard'

export const router = createBrowserRouter([
  { path: '/', element: <BookingLandingPage /> },
  { path: '/login', element: <LoginPage /> },
  { path: '/reservar', element: <BookingFlowPage /> },
  { path: '/reservar/resultado', element: <CheckoutResultPage /> },
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
      { path: 'pacientes', element: <PacientesPage /> },
      { path: 'pacientes/:id', element: <PacienteDetallePage /> },
      { path: 'pagos', element: <PagoPage /> },
      { path: 'notificaciones', element: <NotificacionesPage /> },
    ],
  },
])