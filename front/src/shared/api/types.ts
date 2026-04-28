// ============================================================================
// API Types for NoviVovi Frontend
// Generated from backend API responses
// ============================================================================

// ============================================================================
// Common Types
// ============================================================================

export interface Transform {
  x: number;
  y: number;
  width: number;
  height: number;
  scale: number;
  rotation: number;
  zIndex: number;
}

export interface ImageResponse {
  id: string;
  url: string;
}

// ============================================================================
// Novel Types
// ============================================================================

export interface NovelResponse {
  id: string;
  title: string;
  startLabelId: string;
  labelIds: string[];
  characterIds: string[];
}

export interface CreateNovelRequest {
  title: string;
  description?: string;
}

export interface PatchNovelRequest {
  title?: string;
  description?: string;
}

// ============================================================================
// Label Types
// ============================================================================

export interface LabelResponse {
  id: string;
  name: string;
  novelId: string;
  steps: StepResponse[];
}

export interface AddLabelRequest {
  name: string;
}

export interface PatchLabelRequest {
  name?: string;
}

// ============================================================================
// Character Types
// ============================================================================

export interface CharacterStateResponse {
  id: string;
  name: string;
  description?: string;
  image: ImageResponse;
  localTransform: Transform;
}

export interface CharacterResponse {
  id: string;
  name: string;
  nameColor: string;
  description?: string;
  characterStates: CharacterStateResponse[];
}

export interface AddCharacterRequest {
  name: string;
  nameColor?: string;
  description?: string;
}

export interface PatchCharacterRequest {
  name?: string;
  nameColor?: string;
  description?: string;
}

// ============================================================================
// Step Types (Polymorphic)
// ============================================================================

export interface ReplicaResponse {
  id: string;
  speakerId: string;
  text: string;
}

export interface ChoiceResponse {
  text: string;
  transition: ChoiceTransitionResponse;
}

export interface MenuResponse {
  choices: ChoiceResponse[];
}

// Transition Types
export type TransitionResponse = 
  | NextStepTransitionResponse 
  | JumpTransitionResponse 
  | ChoiceTransitionResponse;

export interface NextStepTransitionResponse {
  type: 'next_step';
}

export interface JumpTransitionResponse {
  type: 'jump';
  targetLabelId: string;
}

export interface ChoiceTransitionResponse {
  type: 'choice';
  targetLabelId: string;
}

// Step Response Types (Polymorphic)
export type StepResponse =
  | ShowReplicaStepResponse
  | ShowMenuStepResponse
  | ShowBackgroundStepResponse
  | ShowCharacterStepResponse
  | HideCharacterStepResponse
  | JumpStepResponse;

export interface ShowReplicaStepResponse {
  type: 'show_replica';
  id: string;
  replica: ReplicaResponse;
  transition: TransitionResponse;
}

export interface ShowMenuStepResponse {
  type: 'show_menu';
  id: string;
  menu: MenuResponse;
  transition: TransitionResponse;
}

export interface ShowBackgroundStepResponse {
  type: 'show_background';
  id: string;
  imageId: string;
  transform: Transform;
  transition: TransitionResponse;
}

export interface ShowCharacterStepResponse {
  type: 'show_character';
  id: string;
  characterId: string;
  characterStateId: string;
  transform: Transform;
  transition: TransitionResponse;
}

export interface HideCharacterStepResponse {
  type: 'hide_character';
  id: string;
  characterId: string;
  transition: TransitionResponse;
}

export interface JumpStepResponse {
  type: 'jump';
  id: string;
  targetLabelId: string;
  transition: TransitionResponse;
}

// Step Request Types
export interface AddStepRequest {
  type: 'show_replica' | 'show_menu' | 'show_background' | 'show_character' | 'hide_character' | 'jump';
  // Replica fields
  speakerId?: string;
  text?: string;
  // Menu fields
  choices?: Array<{
    text: string;
    targetLabelId: string;
  }>;
  // Background fields
  imageId?: string;
  transform?: Transform;
  // Character fields
  characterId?: string;
  characterStateId?: string;
  // Jump fields
  targetLabelId?: string;
}

export interface PatchStepRequest {
  type?: 'show_replica' | 'show_menu' | 'show_background' | 'show_character' | 'hide_character' | 'jump';
  // Replica fields
  speakerId?: string;
  text?: string;
  // Menu fields
  choices?: Array<{
    text: string;
    targetLabelId: string;
  }>;
  // Background fields
  imageId?: string;
  transform?: Transform;
  // Character fields
  characterId?: string;
  characterStateId?: string;
  // Jump fields
  targetLabelId?: string;
}

// ============================================================================
// Graph Types
// ============================================================================

export type NodeResponse = 
  | DefaultNodeResponse 
  | JumpNodeResponse 
  | MenuNodeResponse;

export interface DefaultNodeResponse {
  type: 'default';
  labelId: string;
  labelName: string;
}

export interface JumpNodeResponse {
  type: 'jump';
  labelId: string;
  labelName: string;
}

export interface MenuNodeResponse {
  type: 'choice';
  labelId: string;
  labelName: string;
  choices: Array<{
    text: string;
    targetLabelId: string;
  }>;
}

export type EdgeResponse = JumpEdgeResponse | ChoiceEdgeResponse;

export interface JumpEdgeResponse {
  type: 'jump';
  stepId: string;
  sourceLabelId: string;
  targetLabelId: string;
}

export interface ChoiceEdgeResponse {
  type: 'choice';
  stepId: string;
  sourceLabelId: string;
  targetLabelId: string;
  choiceText: string;
}

export interface NovelGraphResponse {
  nodes: NodeResponse[];
  edges: EdgeResponse[];
}

// ============================================================================
// Type Guards
// ============================================================================

export function isShowReplicaStep(step: StepResponse): step is ShowReplicaStepResponse {
  return step.type === 'show_replica';
}

export function isShowMenuStep(step: StepResponse): step is ShowMenuStepResponse {
  return step.type === 'show_menu';
}

export function isShowBackgroundStep(step: StepResponse): step is ShowBackgroundStepResponse {
  return step.type === 'show_background';
}

export function isShowCharacterStep(step: StepResponse): step is ShowCharacterStepResponse {
  return step.type === 'show_character';
}

export function isHideCharacterStep(step: StepResponse): step is HideCharacterStepResponse {
  return step.type === 'hide_character';
}

export function isJumpStep(step: StepResponse): step is JumpStepResponse {
  return step.type === 'jump';
}

export function isNextStepTransition(transition: TransitionResponse): transition is NextStepTransitionResponse {
  return transition.type === 'next_step';
}

export function isJumpTransition(transition: TransitionResponse): transition is JumpTransitionResponse {
  return transition.type === 'jump';
}

export function isChoiceTransition(transition: TransitionResponse): transition is ChoiceTransitionResponse {
  return transition.type === 'choice';
}
