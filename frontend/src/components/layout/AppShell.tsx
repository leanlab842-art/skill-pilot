import type { ReactNode } from "react";
import { Link, useLocation } from "react-router-dom";
import { LayoutDashboard, LogOut } from "lucide-react";
import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarHeader,
  SidebarInset,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
  SidebarProvider,
  SidebarTrigger,
} from "@/components/ui/sidebar";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { useAuth } from "@/contexts/AuthContext";

const navItems = [{ to: "/", label: "ダッシュボード", icon: LayoutDashboard }];

/**
 * ログイン後の全画面で使う共通レイアウト。左サイドバー(デスクトップ)/ドロワー(モバイル)+
 * メインコンテンツで構成する。
 */
export function AppShell({ children }: { children: ReactNode }) {
  const { user, logout } = useAuth();
  const location = useLocation();

  return (
    <SidebarProvider>
      <Sidebar>
        <SidebarHeader>
          <Link to="/" className="flex items-center gap-2 px-2 py-1.5">
            <span className="flex size-7 items-center justify-center rounded-md bg-primary text-sm font-semibold text-primary-foreground">
              SP
            </span>
            <span translate="no" className="font-semibold">SkillPilot</span>
          </Link>
        </SidebarHeader>
        <SidebarContent>
          <SidebarMenu>
            {navItems.map((item) => (
              <SidebarMenuItem key={item.to}>
                <SidebarMenuButton
                  render={<Link to={item.to} />}
                  isActive={location.pathname === item.to}
                >
                  <item.icon />
                  <span>{item.label}</span>
                </SidebarMenuButton>
              </SidebarMenuItem>
            ))}
          </SidebarMenu>
        </SidebarContent>
        <SidebarFooter>
          <DropdownMenu>
            <DropdownMenuTrigger className="flex w-full items-center gap-2 rounded-md p-2 text-left text-sm hover:bg-sidebar-accent">
              <Avatar className="size-7">
                <AvatarFallback className="text-xs">
                  {user?.name?.charAt(0) ?? "?"}
                </AvatarFallback>
              </Avatar>
              <span className="flex-1 truncate">{user?.name ?? "..."}</span>
            </DropdownMenuTrigger>
            <DropdownMenuContent side="top" align="start" className="w-56">
              <DropdownMenuItem onSelect={() => void logout()}>
                <LogOut />
                ログアウト
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        </SidebarFooter>
      </Sidebar>
      <SidebarInset>
        <header className="flex h-14 items-center gap-2 border-b px-4 md:hidden">
          <SidebarTrigger />
          <span translate="no" className="font-semibold">SkillPilot</span>
        </header>
        <main className="min-w-0 flex-1 p-4 md:p-8">{children}</main>
      </SidebarInset>
    </SidebarProvider>
  );
}
