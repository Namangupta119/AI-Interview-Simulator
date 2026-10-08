import {
    createContext,
    useCallback,
    useContext,
    useEffect,
    useState,
    type ReactNode,
} from "react";
import {
    getCurrentUser,
    login as loginApi,
    type CurrentUserResponse,
    type LoginRequest,
} from "../api/authApi";
import {
    clearAccessToken,
    getAccessToken,
    setAccessToken,
} from "../utils/authStorage";

interface AuthContextValue {
    user: CurrentUserResponse | null;
    isAuthenticated: boolean;
    isLoading: boolean;
    login: (request: LoginRequest) => Promise<void>;
    logout: () => void;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

interface AuthProviderProps {
    children: ReactNode;
}

export function AuthProvider({ children }: AuthProviderProps) {
    const [user, setUser] = useState<CurrentUserResponse | null>(null);
    const [isLoading, setIsLoading] = useState(true);

    const loadCurrentUser = useCallback(async () => {
        const token = getAccessToken();

        if (!token) {
            setIsLoading(false);
            return;
        }

        try {
            const currentUser = await getCurrentUser();
            setUser(currentUser);
        } catch {
            clearAccessToken();
            setUser(null);
        } finally {
            setIsLoading(false);
        }
    }, []);

    useEffect(() => {
        void loadCurrentUser();
    }, [loadCurrentUser]);

    const login = async (request: LoginRequest) => {
        const response = await loginApi(request);

        setAccessToken(response.accessToken);

        const currentUser = await getCurrentUser();
        setUser(currentUser);
    };

    const logout = () => {
        clearAccessToken();
        setUser(null);
    };

    const value: AuthContextValue = {
        user,
        isAuthenticated: user !== null,
        isLoading,
        login,
        logout,
    };

    return (
        <AuthContext.Provider value={value}>
            {children}
        </AuthContext.Provider>
    );
}

export function useAuth(): AuthContextValue {
    const context = useContext(AuthContext);

    if (!context) {
        throw new Error("useAuth must be used within an AuthProvider.");
    }

    return context;
}