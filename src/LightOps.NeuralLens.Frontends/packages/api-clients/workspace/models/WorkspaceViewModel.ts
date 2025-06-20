/* tslint:disable */
/* eslint-disable */
/**
 * Workspace API
 * A Web API for workspaces as part of the LightOps NeuralLens Platform.
 *
 * The version of the OpenAPI document: v1
 * 
 *
 * NOTE: This class is auto generated by OpenAPI Generator (https://openapi-generator.tech).
 * https://openapi-generator.tech
 * Do not edit the class manually.
 */

import { mapValues } from '../runtime';
/**
 * 
 * @export
 * @interface WorkspaceViewModel
 */
export interface WorkspaceViewModel {
    /**
     * 
     * @type {string}
     * @memberof WorkspaceViewModel
     */
    id: string;
    /**
     * 
     * @type {string}
     * @memberof WorkspaceViewModel
     */
    organizationId: string;
    /**
     * 
     * @type {string}
     * @memberof WorkspaceViewModel
     */
    name: string;
    /**
     * 
     * @type {string}
     * @memberof WorkspaceViewModel
     */
    description?: string;
    /**
     * 
     * @type {string}
     * @memberof WorkspaceViewModel
     */
    ingestKey: string;
    /**
     * 
     * @type {Date}
     * @memberof WorkspaceViewModel
     */
    createdAt: Date;
    /**
     * 
     * @type {Date}
     * @memberof WorkspaceViewModel
     */
    updatedAt: Date;
}

/**
 * Check if a given object implements the WorkspaceViewModel interface.
 */
export function instanceOfWorkspaceViewModel(value: object): value is WorkspaceViewModel {
    if (!('id' in value) || value['id'] === undefined) return false;
    if (!('organizationId' in value) || value['organizationId'] === undefined) return false;
    if (!('name' in value) || value['name'] === undefined) return false;
    if (!('ingestKey' in value) || value['ingestKey'] === undefined) return false;
    if (!('createdAt' in value) || value['createdAt'] === undefined) return false;
    if (!('updatedAt' in value) || value['updatedAt'] === undefined) return false;
    return true;
}

export function WorkspaceViewModelFromJSON(json: any): WorkspaceViewModel {
    return WorkspaceViewModelFromJSONTyped(json, false);
}

export function WorkspaceViewModelFromJSONTyped(json: any, ignoreDiscriminator: boolean): WorkspaceViewModel {
    if (json == null) {
        return json;
    }
    return {
        
        'id': json['id'],
        'organizationId': json['organizationId'],
        'name': json['name'],
        'description': json['description'] == null ? undefined : json['description'],
        'ingestKey': json['ingestKey'],
        'createdAt': (new Date(json['createdAt'])),
        'updatedAt': (new Date(json['updatedAt'])),
    };
}

export function WorkspaceViewModelToJSON(json: any): WorkspaceViewModel {
    return WorkspaceViewModelToJSONTyped(json, false);
}

export function WorkspaceViewModelToJSONTyped(value?: WorkspaceViewModel | null, ignoreDiscriminator: boolean = false): any {
    if (value == null) {
        return value;
    }

    return {
        
        'id': value['id'],
        'organizationId': value['organizationId'],
        'name': value['name'],
        'description': value['description'],
        'ingestKey': value['ingestKey'],
        'createdAt': ((value['createdAt']).toISOString()),
        'updatedAt': ((value['updatedAt']).toISOString()),
    };
}

