import { Navigate, Route, Routes } from "react-router-dom";
import { AppShell } from "@/components/layout/AppShell";
import { ProtectedRoute } from "@/routes/ProtectedRoute";
import { LoginPage } from "@/features/auth/pages/LoginPage";
import { RegisterPage } from "@/features/auth/pages/RegisterPage";
import { DashboardPage } from "@/features/dashboard/pages/DashboardPage";
import { JobAnalysisFormPage } from "@/features/job-analysis/pages/JobAnalysisFormPage";
import { JobAnalysisResultPage } from "@/features/job-analysis/pages/JobAnalysisResultPage";
import { LearningRoadmapPage } from "@/features/learning-roadmap/pages/LearningRoadmapPage";

function AppLayout() {
  return (
    <AppShell>
      <Routes>
        <Route index element={<DashboardPage />} />
        <Route path="analyses/new" element={<JobAnalysisFormPage />} />
        <Route path="analyses/:id" element={<JobAnalysisResultPage />} />
        <Route path="analyses/:id/roadmap" element={<LearningRoadmapPage />} />
      </Routes>
    </AppShell>
  );
}

export function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />
      <Route element={<ProtectedRoute />}>
        <Route path="/*" element={<AppLayout />} />
      </Route>
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}
