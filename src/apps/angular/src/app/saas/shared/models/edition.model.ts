export interface EditionDto {
  id: string;
  name: string;
  displayName: string;
  monthlyPrice: number;
  yearlyPrice: number;
  featureLimits: Record<string, any>;
  isActive: boolean;
  creationTime: string;
}

export interface CreateEditionDto {
  name: string;
  displayName: string;
  monthlyPrice: number;
  yearlyPrice: number;
  featureLimits: Record<string, any>;
  isActive: boolean;
}

export interface UpdateEditionDto {
  displayName: string;
  monthlyPrice: number;
  yearlyPrice: number;
  featureLimits: Record<string, any>;
  isActive: boolean;
}
